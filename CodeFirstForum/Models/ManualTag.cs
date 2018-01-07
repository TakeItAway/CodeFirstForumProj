using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodeFirstForum.Models
{
    public class ManualTag
    {
        [Key]
        public int ManualTagId { get; set; }
        public int ManualId { get; set; }
        public int TagId { get; set; }
    }
}
