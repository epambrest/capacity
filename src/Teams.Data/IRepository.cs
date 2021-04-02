using System.Linq;
using System.Threading.Tasks;

namespace Teams.Data
{
    public interface IRepository<T, K> where T : class
    {
        Task<T> GetByIdAsync (K id);
        IQueryable<T> GetAll();
        Task<bool> InsertAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
    }
}
