using System.Linq;
using System.Threading.Tasks;

namespace Teams.Data
{
    interface IRepository<T> where T : class, IQueryable
    {
        T GetAll();
        T GetById(T id);
        Task<T> Insert(T entity);
        Task<T> Update(T entity);
        Task<T> Delete(T entity);
    }
}
