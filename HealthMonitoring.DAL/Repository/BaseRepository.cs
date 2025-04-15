using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Consts;
using HealthMonitoring.DAL.Data.DbHelper;
using HealthMonitoring.DAL.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.DAL.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly HealthMonitoringContext _dbcontext;
        internal DbSet<T> _dbset;
        public BaseRepository( HealthMonitoringContext dbcontext)
        {
            _dbcontext = dbcontext;
            _dbset = _dbcontext.Set<T>();   
        }
        public async Task<IEnumerable<T>>  GetAllAsync(int pagesize = 0,int pagenumber =1)
        {
            IQueryable<T> query = _dbset;
            if (pagesize > 0)
            {
                if (pagesize > 100)
                { pagesize = 100; }
                query = query.Skip(pagesize * (pagenumber - 1)).Take(pagesize);
            }
            return await query.ToListAsync(); 
        }
        public async Task<T> GetByIdAsync(int id) { 
            
           return await _dbset.FindAsync(id)!; 
        }
        public async Task<T> FindAsync(Expression<Func<T, bool>> criteria,string[] includs= null)
        {
            IQueryable<T> query = _dbset;
            if (includs != null)
            {
                foreach (var item in includs)
                { 
                    query = query.Include(item);     
                }

            }
                return await query.SingleOrDefaultAsync(criteria);
        }
        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, int? take, int? skip,
            string[] includs=null,Expression<Func<T, object>>ordereby=null,string orderbydirection=OrderBy.Ascending)
        {
            IQueryable<T> query = _dbset.Where(criteria);
            if (take.HasValue)
                query = query.Take(take.Value);
            if (skip.HasValue)
                query = query.Skip(skip.Value);
            if (includs != null)
            {
                foreach (var item in includs)
                {
                    query = query.Include(item);
                }
            }
            if (ordereby != null)
            {
                if (orderbydirection == OrderBy.Ascending)
                {
                    query = query.OrderBy(ordereby);
                }
                else 
                {
                    query = query.OrderByDescending(ordereby);    
                }
            }
            return await query.ToListAsync();

        }
        public async Task CreateAsync(T entity)
        {
            await _dbset.AddAsync(entity);
        }
        public async Task AddrangeAsync(T entity)
        {
            await _dbset.AddRangeAsync(entity);
        }
        public async Task RemoveAsync(T entity)
        {
            _dbset.Remove(entity);
        }
        public async Task<T> UpdateAsync(T entity)
        {
              _dbset.Update(entity);
               return entity; 
        }
    }
}
