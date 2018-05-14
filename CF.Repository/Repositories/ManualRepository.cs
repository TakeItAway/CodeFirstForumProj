using CF.Data;
using CF.Data.Models;
using CF.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OC.Repository.Repositories
{
    public class ManualRepository : IRepository<Manual>
    {
        private ApplicationDbContext _context;

        public ManualRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Create(Manual item)
        {
            _context.Add(item);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var manual = _context.Manuals.Find(id);
            _context.Remove(manual);
        }

        public Manual GetById(int id)
        {
            return _context.Manuals.Find(id);
        }

        public IEnumerable<Manual> GetList()
        {
            return _context.Manuals;
        }

        public void Update(Manual item)
        {
            _context.Manuals.Update(item);
        }

        public IEnumerable<Manual> GetTagManuals(int tagId)
        {
            List<Manual> list = new List<Manual>();
            List<ManualTag> manualTags = _context.ManualTags.Where(manual => manual.TagId == tagId).ToList();
            foreach (ManualTag manual in manualTags)
            {
                list.Add(_context.Manuals.Find(manual.ManualId));
            }
            return list;
        }
    }
}
