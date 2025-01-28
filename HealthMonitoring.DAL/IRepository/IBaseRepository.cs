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
         Task<IEnumerable<T>> GetAllAsync();
         Task<T> GetByIdAsync(int id);
         Task<T> FindAsync(Expression<Func<T, bool>> Match, string[] includs = null);
         Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> Match,int? take,int? skip, string[] includs = null,
                                    Expression<Func<T,Object>> ordereby= null,string orderbydirection = OrderBy.Ascending  );

    }
}
