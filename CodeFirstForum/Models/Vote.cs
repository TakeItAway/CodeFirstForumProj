﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodeFirstForum.Models
{
    public class Vote
    {
        [Key]
        public int VoteId { get; set; }
        public int CommentId { get; set; }
        public int UserId { get; set; }
    }
}
