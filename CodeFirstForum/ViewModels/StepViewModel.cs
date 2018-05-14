using CF.Data;
using CF.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodeFirstForum.ViewModels
{
    public class StepViewModel
    {
        [Key]
        public Step Step { get; set; }
        public Manual Manual { get; set; }
        public List<Like> Likes { get; set; }
        public List<Comment> Comments { get; set; }
        public ApplicationUser User { get; set; }
        public List<string> Tags { get; set; }
        public ApplicationDbContext Context { get; set; }
        public string Photo { get; set; }
        public string Video { get; set; }
    }
}
