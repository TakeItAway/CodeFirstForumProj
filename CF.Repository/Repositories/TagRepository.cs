using CF.Data;
using CF.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CF.Repository.Repositories
{
    public class TagRepository : IRepository<Tag>
    {
        private ApplicationDbContext _context;

        public TagRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Create(Tag item)
        {
            _context.Tags.Add(item);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var tag = _context.Tags.Find(id);
            _context.Remove(tag);
        }

        public Tag GetById(int id)
        {
            return _context.Tags.Find(id);
        }

        public IEnumerable<Tag> GetList()
        {
            return _context.Tags;
        }

        public IEnumerable<Tag> GetManualTags(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Tag item)
        {
            _context.Update(item);
        }
    }
}