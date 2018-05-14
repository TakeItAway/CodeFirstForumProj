using CF.Data;
using CF.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodeFirstForum.ViewModels
{
    public class ManualViewModel
    {
        [Key]
        public Manual Manual { get; set; }
        public List<Like> Likes { get; set; }
        public List<Comment> Comments { get; set; }
        public ApplicationUser User { get; set; }
        public List<string> Tags { get; set; }   
        public List<Step> Steps { get; set; }
        public ECategory Category { get; set; }
        public ApplicationDbContext Context { get; set; }
    }
}
