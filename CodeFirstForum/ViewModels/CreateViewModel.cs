using CF.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodeFirstForum.ViewModels
{
    public class CreateViewModel
    {
        [Required]
        public string AuthorId { get; set; }
        public Manual Manual { get; set; }
        public List<string> Tags { get; set; }
        public List<Step> Steps { get; set; }
    }
}
