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
        private readonly HealthMonitoringContext _dbcontext;
        internal DbSet<T> _dbset;

        public BaseRepository( HealthMonitoringContext dbcontext)
        {
            _dbcontext = dbcontext;
            _dbset = _dbcontext.Set<T>();   
        }
        public async Task<IEnumerable<T>>  GetAllAsync()
        {
            IQueryable<T> query = _dbset;
            return await query.ToListAsync(); 
        }
        public async Task<T> GetByIdAsync(int id)=> await _dbset.FindAsync(id);
        public async Task<T> FindAsync(Expression<Func<T, bool>> Match,string[] includs= null)
        {
            IQueryable<T> query = _dbset;
            if (includs != null)
            {
                foreach (var item in includs)
                { 
                    query = query.Include(item);     
                }

            }
                return await query.SingleOrDefaultAsync(Match);
        }

    

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> Match, int? take, int? skip, string[] includs = null,
            Expression<Func<T, object>> ordereby = null, string orderbydirection = OrderBy.Ascending)
        {
            IQueryable<T> query = _dbset.Where(Match);
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
    }
}
