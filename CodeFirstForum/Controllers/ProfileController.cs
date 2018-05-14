using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CF.Data;
using CF.Data.Models;
using CodeFirstForum.ViewModels;
using HeyRed.MarkdownSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CodeFirstForum.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext context;
        Markdown mark = new Markdown(new MarkdownOptions
        {
            AutoHyperlink = true,
            AutoNewLines = true,
            LinkEmails = true,
            QuoteSingleLine = true,
            StrictBoldItalic = true
        });

        public ProfileController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager, ApplicationDbContext dbContext)

        {
            _userManager = userManager;
            _signInManager = signInManager;
            context = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            List<Manual> toDelete = context.Manuals.Where(i => i.Saved == false).ToList();
            foreach (Manual man in toDelete)
            {
                DelEmptyTags(man.ManualId, context);
                context.Manuals.Remove(man);
            }
            context.SaveChanges();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            return View(user);
        }
        [HttpGet]
        [HttpPost]
        public ActionResult Create(string id)
        {
            CreateViewModel model = new CreateViewModel()
            {
                Manual = null,
                AuthorId = id,
                Steps = new List<Step>(),
                Tags = new List<string>()
            };
            return View(model);
        }
        [HttpPost]
        [ActionName("CreateManual")]
        public ActionResult CreateManual(CreateManualViewModel model)
        {
            Manual manual = new Manual()
            {
                AuthorId = model.AuthorId,
                Title = mark.Transform(model.Title),
                Description = mark.Transform(model.Description),
                Photo = model.Photo,
                Category = model.Category,
                ReleaseDate = DateTime.Today,
                LastUpdate = DateTime.Today
            };
            EntityEntry<Manual> e = context.Manuals.Add(manual);
            context.SaveChanges();
            Manual i = e.Entity;
            string[] tags = model.Tags.Split(',');
            foreach (string t in tags)
            {
                Tag fTag = null;

                if (context.Tags.Where(dt => dt.Name == t).ToList().Count != 0)
                {
                    fTag = context.Tags.Where(dt => dt.Name == t).ToList().First();
                }

                if (fTag != null)
                {
                    context.ManualTags.Add(new ManualTag() { TagId = fTag.TagId, ManualId = i.ManualId });
                }
                else
                {
                    EntityEntry<Tag> added = context.Tags.Add(new Tag() { Name = t });
                    context.SaveChanges();
                    Tag aTag = added.Entity;
                    context.ManualTags.Add(new ManualTag() { TagId = aTag.TagId, ManualId = i.ManualId });
                }
            }
            context.SaveChanges();
            CreateViewModel newModel = new CreateViewModel()
            {
                Manual = i,
                AuthorId = i.AuthorId,
                Tags = tags.ToList(),
                Steps = new List<Step>()
            };
            return View("Create", newModel);
        }

        [HttpPost]
        public ActionResult SaveAll(int id)
        {
            Manual toSave = context.Manuals.Find(id);
            toSave.Saved = true;
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [HttpGet]
        [ActionName("Delete")]
        public ActionResult DeleteManual(int id)
        {
            Manual delMan = context.Manuals.Find(id);
            context.Manuals.Remove(delMan);
            DelEmptyTags(id, context);
            List<ManualTag> manualTags = ManualHelper.GetManualTags(id, context);
            context.Comments.RemoveRange(ManualHelper.GetManualComments(id, context));
            context.Steps.RemoveRange(ManualHelper.GetManualSteps(id, context));
            context.ManualTags.RemoveRange(manualTags);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [HttpGet]
        [ActionName("Edit")]
        public ActionResult EditManual(int id)
        {
            Manual editMan = context.Manuals.Find(id);
            CreateViewModel model = new CreateViewModel()
            {
                AuthorId = editMan.AuthorId,
                Manual = editMan,
                Steps = context.Steps.Where(s => s.ManualId == editMan.ManualId).ToList()
            };
            return View("EditManual", model);
        }

        [HttpPost]
        [ActionName("AddStep")]
        public ActionResult AddStep(AddStepViewModel model)
        {
            Step step = new Step()
            {
                ManualId = model.ManualId,
                Number = model.Number,
                Title = mark.Transform(model.Title),
                Content = mark.Transform(model.Content),
                Photo = model.Photo,
                Video = model.Video
            };

            context.Steps.Add(step);
            context.SaveChanges();

            if (step.Number != 0)
            {
                List<Step> steps = context.Steps.Where(t => t.ManualId == model.ManualId).ToList();
                Step prevStep = steps.Find(x => x.Number == model.Number - 1);
                Step firstStep = steps.Find(x => x.Number == 0);
                firstStep.PrevId = step.StepId;
                step.NextId = firstStep.StepId;
                prevStep.NextId = step.StepId;
                step.PrevId = prevStep.StepId;
            }
            else
            {
                step.PrevId = step.StepId;
                step.NextId = step.StepId;
            }
            context.SaveChanges();

            List<ManualTag> instrTags = context.ManualTags.Where(it => it.ManualId == model.ManualId).ToList();
            List<string> tags = new List<string>();
            foreach (ManualTag it in instrTags)
            {
                Tag t = context.Tags.Find(it.TagId);
                tags.Add(t.Name);
            }

            Manual instr = context.Manuals.Find(model.ManualId);

            return View("Create", new CreateViewModel()
            {
                Manual = instr,
                AuthorId = instr.AuthorId,
                Steps = context.Steps.Where(s => s.ManualId == instr.ManualId).ToList(),
                Tags = tags
            });
        }
        [HttpPost]
        public ActionResult EditManualHead(string authorId, int manualId, string title, string descr, string photo)
        {
            Manual editMan = context.Manuals.Find(manualId);
            editMan.Title = mark.Transform(title);
            editMan.Description = mark.Transform(descr);
            editMan.Photo = photo;
            editMan.LastUpdate = DateTime.Today;

            context.Manuals.Update(editMan);
            context.SaveChanges();
            CreateViewModel model = new CreateViewModel()
            {
                AuthorId = editMan.AuthorId,
                Manual = editMan,
                Steps = context.Steps.Where(s => s.ManualId == editMan.ManualId).ToList()
            };
            return View("EditManual", model);
        }

        [HttpPost]
        public ActionResult EditStep(int stepId, string authorId, string title, string content, string photo)
        {
            Step editStep = context.Steps.Find(stepId);
            editStep.Title = mark.Transform(title);
            editStep.Content = mark.Transform(content);

            context.Steps.Update(editStep);
            context.SaveChanges();
            CreateViewModel model = new CreateViewModel()
            {
                AuthorId = authorId,
                Manual = context.Manuals.Find(editStep.ManualId),
                Steps = context.Steps.Where(s => s.ManualId == editStep.ManualId).ToList()
            };
            return View("EditManual", model);
        }
        private void DelEmptyTags(int id, ApplicationDbContext context)
        {
            List<ManualTag> manualTags = ManualHelper.GetManualTags(id, context);
            foreach (ManualTag mTag in manualTags)
            {
                if (ManualHelper.CountSameNameManualTags(mTag.ManualTagId, context) < 2)
                {
                    context.Tags.Remove(context.Tags.Find(mTag.TagId));
                }
            }
            context.SaveChanges();
        }
    }
}