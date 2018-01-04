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
            List<ManualTag> manTags = context.ManualTags.Where(t => t.ManualId == id).ToList();
            List<string> tags = new List<string>();
            foreach (ManualTag it in manTags)
            {
                tags.Add(context.Tags.Find(it.TagId).Name);
            }
            ManualViewModel model = new ManualViewModel
            {
                Manual = manual,
                Comments = context.Comments.Where(c => c.ManualId == manual.ManualId).ToList(),
                User = user,
                Context = context,
                Tags = tags
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
                Tags = tags.ToList()
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
            Manual delInstr = context.Manuals.Find(id);
            context.Manuals.Remove(delInstr);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [HttpGet]
        [ActionName("Edit")]
        public ActionResult EditManual(int id)
        {
            Manual editInstr = context.Manuals.Find(id);
            CreateViewModel model = new CreateViewModel()
            {
                AuthorId = editInstr.AuthorId,
                Manual = editInstr,
                //Steps = context.Steps.Where(s => s.InstructionId == editInstr.InstructionId).ToList()

            };
            return View("EditInstruction", model);
        }

        [HttpPost]
        public ActionResult EditManualHead(string authorId, int instrId, string title, string descr, string photo)
        {
            Manual editInstr = context.Manuals.Find(instrId);
            editInstr.AuthorId = authorId;
            editInstr.Title = title;
            editInstr.Description = descr;
            editInstr.Photo = photo;

            context.Manuals.Update(editInstr);

            CreateViewModel model = new CreateViewModel()
            {
                AuthorId = editInstr.AuthorId,
                Manual = editInstr,
                //Steps = context.Steps.Where(s => s.ManualId == editInstrManualId).ToList()

            };
            return View("EditInstruction", model);
        }

        //[HttpPost]
        //public ActionResult EditStep(int stepId, string authorId, int number, string title, string content, string photo)
        //{
        //    Step editStep = context.Steps.Find(stepId);
        //    editStep.Number = number;
        //    editStep.Title = title;
        //    editStep.Content = content;
        //    editStep.Photo = photo;

        //    context.Steps.Update(editStep);

        //    CreateViewModel model = new CreateViewModel()
        //    {
        //        AuthorId = authorId,
        //        Instruction = context.Instructions.Find(editStep.InstructionId),
        //        Steps = context.Steps.Where(s => s.InstructionId == editStep.InstructionId).ToList()

        //    };
        //    return View("EditInstruction", model);
        //}


    }
}