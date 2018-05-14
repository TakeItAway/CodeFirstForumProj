using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CF.Data.Models
{
    public class Like
    {
        [Key]
        public int LikeId { get; set; }
        public int ManualId { get; set; }
        public int UserId { get; set; }
        public int Points { get; set; }
    }
}
