using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CF.Data.Models
{
    public class Step
    {
        [Key]
        public int StepId { get; set; }
        public int ManualId { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int NextId { get; set; }
        public int PrevId { get; set; }
        public string Photo { get; set; }
        public string Video { get; set; }
    }
}
