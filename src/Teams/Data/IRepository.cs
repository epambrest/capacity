using System.Linq;
using System.Threading.Tasks;

namespace Teams.Data
{
    interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        T GetById(T Id);
        Task<T> InsertAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(T entity);
    }
}
