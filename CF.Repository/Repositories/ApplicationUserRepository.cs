using CF.Data;
using CF.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CF.Repository.Repositories
{
    public class ApplicationUserRepository : IRepository<ApplicationUser>
    {
        private ApplicationDbContext _context;

        public ApplicationUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Create(ApplicationUser item)
        {
            _context.Add(item);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            //_context.Remove
        }

        public ApplicationUser GetById(int id)
        {
            throw new NotImplementedException();
            // return _context.ApplicationUsers.Where(user => user)
        }

        public IEnumerable<ApplicationUser> GetList()
        {
            return _context.ApplicationUsers.Where(u => true);
        }

        public void Update(ApplicationUser item)
        {
            throw new NotImplementedException();
        }
    }
}