using CF.Data;
using CF.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeFirstForum.ViewModels
{
    public class HomeIndexViewModel
    {
        public int? TagId { get; set; }
        public List<Manual> Manuals { get; set; }
        public List<Tag> Tags { get; set; }
        public ApplicationDbContext Context { get; set; }
    }
}
