using Microsoft.AspNetCore.SignalR;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace Teams.Data
{
    interface IRepository<T, K> where T : class
    {
        Task<T> GetByIdAsync (K id);
        Task<IQueryable<T>> GetAll();
        Task<T> InsertAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(T entity);
    }
}
