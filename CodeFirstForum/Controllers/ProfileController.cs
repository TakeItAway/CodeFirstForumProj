using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeFirstForum.Data;
using CodeFirstForum.Models;
using CodeFirstForum.ViewModels;
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
                Title = model.Title,
                Description = model.Description,
                Photo = model.Photo
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
            editMan.Title = title;
            editMan.Description = descr;
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
                Content = content,
                ManualId = manualId
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
                Title = model.Title,
                Content = model.Content
            };

            context.Steps.Add(step);
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
        public ActionResult EditStep(int stepId, string authorId, int number, string title, string content, string photo)
        {
            Step editStep = context.Steps.Find(stepId);
            editStep.Number = number;
            editStep.Title = title;
            editStep.Content = content;

            context.Steps.Update(editStep);

            CreateViewModel model = new CreateViewModel()
            {
                AuthorId = authorId,
                Manual = context.Manuals.Find(editStep.ManualId),
                Steps = context.Steps.Where(s => s.ManualId == editStep.ManualId).ToList()

            };
            return View("EditManual", model);
        }
    }
}