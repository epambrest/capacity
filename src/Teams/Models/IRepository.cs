using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Teams.Models
{
    interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(IQueryable<T> id);
        bool InsertAsync(T obj);
        bool UpdateAsync(T obj);
        bool DeleteAsync(object id);
        bool SaveAsync();
    }
}
