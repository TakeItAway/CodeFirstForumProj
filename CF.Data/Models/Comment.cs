using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CF.Data.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public string AuthorId { get; set; }
        public int ManualId { get; set; }
        public string Content { get; set; }
        public int VoteCount { get; set; }
    }
}
