using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Teams.Data.Models;

namespace Teams.Data.Repository
{
    public class Repository<TDb, TDomain, K> : IRepository<TDb, TDomain, int> 
        where TDb : class
        where TDomain : class
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        DbSet<TDb> _dbSet;
        
        public Repository(ApplicationDbContext dbcontext, IMapper mapper)
        {
            _dbContext = dbcontext;
            _mapper = mapper;
            _dbSet = dbcontext.Set<TDb>();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            TDb deletedItem = await _dbSet.FindAsync(id);

            if (deletedItem is Team)
            {
                await Team.DeleteDependEntentities(_dbContext, id);
            }

            _dbSet.Remove(deletedItem);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<IEnumerable<TDomain>> GetAllAsync()
        {
            List<TDb> allItems = await _dbSet.ToListAsync();
            return _mapper.Map<List<TDb>, IEnumerable<TDomain>>(allItems);
        }

        public async Task<TDomain> GetByIdAsync(int id)
        {
            TDb tDb = await _dbSet.FindAsync(id);
            return _mapper.Map<TDomain>(tDb);
        }

        public async Task<bool> InsertAsync(TDomain entity)
        {
            TDb tDb = _mapper.Map<TDb>(entity);
            await _dbSet.AddAsync(tDb);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }
        public async Task<bool> UpdateAsync(TDomain entity)
        {
            IModel<TDb> entityDb = (IModel<TDb>)_mapper.Map<TDb>(entity);
            TDb currentItem = await _dbSet.FindAsync(entityDb.Id);
            
            if (currentItem == null) return false;

            IModel<TDb> updatedItem = (IModel<TDb>)currentItem;

            updatedItem.Update((TDb)entityDb);

            var result = _dbContext.Entry(updatedItem).State == EntityState.Modified ? true : false;
            await _dbContext.SaveChangesAsync();
            return result;
        }
    }
}
