using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeFirstForum.Data;
using CodeFirstForum.Models;
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

        // GET: Profile
        public async Task<IActionResult> Index()
        {
            List<Manual> toDelete = context.Manuals.Where(i => i.Saved == false).ToList();
            context.Manuals.RemoveRange(toDelete);
            context.SaveChanges();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Manual(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            Manual manual = context.Manuals.Find(id);
            ManualViewModel model = new ManualViewModel
            {
                Manual = manual,
                Steps = context.Steps.Where(s => s.ManualId == manual.ManualId).ToList(),
                Comments = context.Comments.Where(c => c.ManualId == manual.ManualId).ToList(),
                User = user,
                Context = context,
                Tags = ManualHelper.GetAllManualTags(id, context)
            };
            return View(model);
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
                Category = model.Category
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
            List<ManualTag> manualTags = ManualHelper.GetManualTags(id, context);
            foreach (ManualTag mTag in manualTags)
            {
                if (ManualHelper.CountSameNameManualTags(mTag.ManualTagId, context) < 2)
                {
                    context.Tags.Remove(context.Tags.Find(mTag.TagId));
                }
            }
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
        public ActionResult EditManualHead(string authorId, int manId, string title, string descr, string photo)
        {
            Manual editMan = context.Manuals.Find(manId);
            editMan.AuthorId = authorId;
            editMan.Title = mark.Transform(title);
            editMan.Description = mark.Transform(descr);
            editMan.Photo = photo;

            context.Manuals.Update(editMan);

            CreateViewModel model = new CreateViewModel()
            {
                AuthorId = editMan.AuthorId,
                Manual = editMan,
                Steps = context.Steps.Where(s => s.ManualId == editMan.ManualId).ToList()

            };
            return View("EditManual", model);
        }

        [HttpPost]
        public ActionResult AddComment(string content, string userId, int manualId)
        {
           
            Comment toAdd = new Comment()
            {
                AuthorId = userId,
                Content = mark.Transform(content),
                ManualId = manualId,
                VoteCount = 0
            };
            context.Comments.Add(toAdd);
            context.SaveChanges();
            ManualViewModel model = new ManualViewModel()
            {
                Manual = context.Manuals.Find(manualId),
                User = context.ApplicationUsers.Find(userId),
                Steps = context.Steps.Where(s => s.ManualId == manualId).ToList(),
                Tags = ManualHelper.GetAllManualTags(manualId, context),
                Comments = context.Comments.Where(c => c.ManualId == manualId).ToList(),
                Context = context
            };
            return View("Manual", model);
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
                Content = mark.Transform(model.Content)
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

        [HttpGet]
        public async Task<IActionResult> Step(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            Step step = context.Steps.Find(id);
            Manual manual = context.Manuals.Find(step.ManualId);
            StepViewModel model = new StepViewModel
            {
                Step = step,
                Manual = manual,
                Comments = context.Comments.Where(c => c.ManualId == manual.ManualId).ToList(),
                User = user,
                Context = context,
                Tags = ManualHelper.GetAllManualTags(manual.ManualId, context)
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult EditStep(int stepId, string authorId, int number, string title, string content, string photo)
        {
            Step editStep = context.Steps.Find(stepId);
            editStep.Number = number;
            editStep.Title = mark.Transform(title);
            editStep.Content = mark.Transform(content);

            context.Steps.Update(editStep);

            CreateViewModel model = new CreateViewModel()
            {
                AuthorId = authorId,
                Manual = context.Manuals.Find(editStep.ManualId),
                Steps = context.Steps.Where(s => s.ManualId == editStep.ManualId).ToList()
            };
            return View("EditManual", model);
        }

        [HttpPost]
        [ActionName("LikeComment")]
        public ActionResult LikeComment(string userId, int commentId)
        {
            //Comment comment = context.Comments.Find(commentId);
            //comment.VoteCount += 1;
            //Manual manual = context.Manuals.Find(comment.ManualId);
            //int manualId = manual.ManualId;
            //context.SaveChanges();
            //ManualViewModel model = new ManualViewModel()
            //{
            //    Manual = manual,
            //    User = context.ApplicationUsers.Find(userId),
            //    Steps = context.Steps.Where(s => s.ManualId == manualId).ToList(),
            //    Tags = ManualHelper.GetAllManualTags(manualId, context),
            //    Comments = context.Comments.Where(c => c.ManualId == manualId).ToList(),
            //    Context = context
            //};
            return View("Test");
        }
        public IActionResult Test(int userId)
        {
            TestViewModel model = new TestViewModel()
            {
                ID = userId
            };
            return View(model);
        }
    }
}