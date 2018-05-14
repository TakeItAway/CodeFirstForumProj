using CF.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeFirstForum.ViewModels
{
    public class ProfileIndexViewModel
    {
        public ApplicationUser User { get; set; }
        public List<Manual> Manuals { get; set; }
    }
}
