﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodeFirstForum.Models
{
    public class Step
    {
        [Key]
        public int StepId { get; set; }
        public int ManualId { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}