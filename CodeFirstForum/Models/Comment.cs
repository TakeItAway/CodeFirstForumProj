﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodeFirstForum.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public string AuthorId { get; set; }
        public int ManualId { get; set; }
        public string Content { get; set; }
    }
}