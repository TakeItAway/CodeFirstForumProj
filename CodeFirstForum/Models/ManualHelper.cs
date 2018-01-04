using CodeFirstForum.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeFirstForum.Models
{
    sealed public class ManualHelper
    {
        public static List<string> GetAllManualTags(int manualId, ApplicationDbContext context)
        {
            Manual manual = context.Manuals.Find(manualId);
            List<ManualTag> manualTags = context.ManualTags.Where(t => t.ManualId == manualId).ToList();
            List<string> tags = new List<string>();
            foreach (ManualTag man in manualTags)
            {
                tags.Add(context.Tags.Find(man.TagId).Name);
            }
            return tags;
        }
    }
}
