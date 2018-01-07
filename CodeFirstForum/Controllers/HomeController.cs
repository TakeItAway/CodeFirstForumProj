using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CodeFirstForum.Models;
using CodeFirstForum.ViewModels;
using Microsoft.AspNetCore.Identity;
using CodeFirstForum.Data;

namespace CodeFirstForum.Controllers
{
    public class HomeController : Controller
    {
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
        public IActionResult Manual()
        {
            return View();
        }
    }
}
