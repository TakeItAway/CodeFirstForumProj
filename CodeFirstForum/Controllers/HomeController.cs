using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CodeFirstForum.ViewModels;
using Microsoft.AspNetCore.Identity;
using HeyRed.MarkdownSharp;
using CF.Data.Models;
using CF.Data;

namespace CodeFirstForum.Controllers
{
    public class HomeController : Controller
    {
        Markdown mark = new Markdown(new MarkdownOptions
        {
            AutoHyperlink = true,
            AutoNewLines = true,
            LinkEmails = true,
            QuoteSingleLine = true,
            StrictBoldItalic = true
        });
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext context;


        public HomeController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager, ApplicationDbContext dbContext)

        {
            _userManager = userManager;
            _signInManager = signInManager;
            context = dbContext;
        }

        public IActionResult Index(int? tagId = null)
        {
            List<Manual> list = new List<Manual>();

            if (tagId != null)
            {
                List<ManualTag> manualTags = context.ManualTags.Where(manual => manual.TagId == tagId).ToList();
                foreach (ManualTag manual in manualTags)
                {
                    list.Add(context.Manuals.Find(manual.ManualId));
                }
                list.Reverse();
                HomeIndexViewModel newModel = new HomeIndexViewModel()
                {
                    TagId = tagId,
                    Tags = context.Tags.ToList(),
                    Manuals = list,
                    Context = context
                };
                return View(newModel);
            }
            else
            {
                list = context.Manuals.ToList();
                HomeIndexViewModel newModel = new HomeIndexViewModel()
                {
                    TagId = null,
                    Tags = context.Tags.ToList(),
                    Manuals = list,
                    Context = context
                };
                return View(newModel);
            }
        }
        public IActionResult ShowCategoryManuals(int category)
        {
            List<Manual> manualList = context.Manuals.Where(manual => Convert.ToInt32(manual.Category) == category).ToList();
            HomeIndexViewModel newModel = new HomeIndexViewModel()
            {
                TagId = null,
                Tags = context.Tags.ToList(),
                Manuals = manualList,
                Context = context
            };
            return View("Index", newModel);
        }

        public IActionResult LikeComment(string userId, int commentId, string path)
        {
            Comment comment = context.Comments.Find(commentId);

            if (context.Votes.Where(c => c.CommentId == commentId && c.UserId == userId).ToList().Count == 0)
            {
                comment.VoteCount++;
                Vote vote = new Vote()
                {
                    CommentId = commentId,
                    UserId = userId,
                };
                context.Votes.Add(vote);
            }
            Manual manual = context.Manuals.Find(comment.ManualId);
            context.SaveChanges();
            ManualViewModel model = new ManualViewModel()
            {
                Manual = manual,
                User = context.ApplicationUsers.Find(userId),
                Steps = context.Steps.Where(s => s.ManualId == manual.ManualId).ToList(),
                Tags = ManualHelper.GetAllManualTags(manual.ManualId, context),
                Comments = context.Comments.Where(c => c.ManualId == manual.ManualId).ToList(),
                Context = context
            };
            return View("Manual", model);
        }

        [HttpPost]
        public ActionResult AddComment(string content, string userId, int manualId)
        {
            Comment toAdd = new Comment()
            {
                AuthorId = userId,
                Content = mark.Transform(content),
                ManualId = manualId,
                VoteCount = 0,
            };
            Manual manual = context.Manuals.Find(manualId);
            manual.LastUpdate = DateTime.Today;
            context.Comments.Add(toAdd);
            context.SaveChanges();
            ManualViewModel model = new ManualViewModel()
            {
                Manual = manual,
                User = context.ApplicationUsers.Find(userId),
                Steps = context.Steps.Where(s => s.ManualId == manualId).ToList(),
                Tags = ManualHelper.GetAllManualTags(manualId, context),
                Comments = context.Comments.Where(c => c.ManualId == manualId).ToList(),
                Context = context
            };
            return View("Manual", model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        public async Task<IActionResult> Manual(int id)
        {
            var user = await _userManager.GetUserAsync(User);
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
        public async Task<IActionResult> Step(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            Step step = context.Steps.Find(id);
            Manual manual = context.Manuals.Find(step.ManualId);
            StepViewModel model = new StepViewModel
            {
                Step = step,
                Manual = manual,
                Comments = context.Comments.Where(c => c.ManualId == manual.ManualId).ToList(),
                User = user,
                Context = context,
                Tags = ManualHelper.GetAllManualTags(manual.ManualId, context),
            };
            return View(model);
        }
    }
}