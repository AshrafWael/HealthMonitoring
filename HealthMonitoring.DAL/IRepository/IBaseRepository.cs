using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Consts;

namespace HealthMonitoring.DAL.IRepository
{
    public interface IBaseRepository<T> where T : class
    {
         Task<IEnumerable<T>> GetAllAsync(int pagesize = 0, int pagenumber = 1);
         Task<T> GetByIdAsync(int id);
         Task<T> FindAsync(Expression<Func<T, bool>> criteria, string[] includs = null);
         Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria,int? take,int? skip, string[] includs = null,
                           Expression<Func<T,Object>>ordereby=null,string orderbydirection=OrderBy.Ascending);
         Task CreateAsync(T entity);
         Task<T> UpdateAsync(T entity);
         Task RemoveAsync(T entity);
        public  Task AddrangeAsync(T entity);
        Task<T> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task DeleteAsync(object id);
        Task<bool> ExistsAsync(object id);




    }
}
