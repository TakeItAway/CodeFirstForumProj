using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CF.Data.Models
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
        public static List<Step> GetManualSteps(int manualId, ApplicationDbContext context)
        {
            Manual manual = context.Manuals.Find(manualId);
            List<Step> manualSteps = context.Steps.Where(t => t.ManualId == manualId).ToList();
            return manualSteps;
        }
        public static List<Comment> GetManualComments(int manualId, ApplicationDbContext context)
        {
            Manual manual = context.Manuals.Find(manualId);
            List<Comment> manualComments = context.Comments.Where(t => t.ManualId == manualId).ToList();
            return manualComments;
        }

        public static List<ManualTag> GetManualTags(int manualId, ApplicationDbContext context)
        {
            Manual manual = context.Manuals.Find(manualId);
            List<ManualTag> manualTags = context.ManualTags.Where(t => t.ManualId == manualId).ToList();
            return manualTags;
        }
        public static int CountSameNameManualTags(int manualTagId, ApplicationDbContext context)
        {
            ManualTag manualTag = context.ManualTags.Find(manualTagId);
            Tag tag = context.Tags.Find(manualTag.TagId);
            List<ManualTag> manualTags = context.ManualTags.Where(t => t.TagId == tag.TagId).ToList();
            return manualTags.Count();
        }
    }
}
