using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodeFirstForum.ViewModels
{
    public class AddStepViewModel
    {
        [Key]
        public int StepId { get; set; }
        [Required]
        public int ManualId { get; set; }
        [Required]
        public int Number { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public string Photo { get; set; }
        public string Video { get; set; }
    }
}
