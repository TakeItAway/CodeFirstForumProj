
using CF.Data;
using CF.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CF.Repository.Repositories
{
    public class LikeRepository : IRepository<Like>
    {
        private ApplicationDbContext _context;

        public LikeRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Create(Like item)
        {
            _context.Likes.Add(item);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var like = _context.Likes.Find(id);
            _context.Likes.Remove(like);
        }

        public Like GetById(int id)
        {
            return _context.Likes.Find(id);
        }

        public IEnumerable<Like> GetList()
        {
            return _context.Likes;
        }

        public IEnumerable<Like> GetManualLikes(int id)
        {
            return _context.Likes.Where(like => like.ManualId == id);
        }

        public void Update(Like item)
        {
            _context.Likes.Update(item);
        }
    }
}