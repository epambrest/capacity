using System.Collections.Generic;
using System.Threading.Tasks;

namespace Teams.Business.Repository
{
    public interface IRepository<T, K> where T : class
    {
        Task<T> GetByIdAsync (K id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<bool> InsertAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
    }
}
