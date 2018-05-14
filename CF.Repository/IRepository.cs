using System;
using System.Collections.Generic;
using System.Text;

namespace CF.Repository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetList();
        T GetById(int id);
        void Create(T item);
        void Update(T item);
        void Delete(int id);
    }
}
