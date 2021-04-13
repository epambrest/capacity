using System.Collections.Generic;
using System.Threading.Tasks;

namespace Teams.Business.Repository
{
    public interface IRepository<TDomain, K> where TDomain : class
    {
        Task<TDomain> GetByIdAsync (K id);
        Task<IEnumerable<TDomain>> GetAllAsync();
        Task<bool> InsertAsync(TDomain entity);
        Task<bool> UpdateAsync(TDomain entity);
        Task<bool> DeleteAsync(K id);
    }
}