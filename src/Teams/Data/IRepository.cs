using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace Teams.Data
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        Task <T> GetById { get; set;}
        Task<T> InsertAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(T entity);
    }
}
