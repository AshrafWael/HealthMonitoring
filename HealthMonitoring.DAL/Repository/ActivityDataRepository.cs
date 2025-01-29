using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.DbHelper;
using HealthMonitoring.DAL.Data.Models;
using HealthMonitoring.DAL.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.DAL.Repository
{
    public class ActivityDataRepository : BaseRepository<ActivityData> ,IActivityDataRepository
    {
        private readonly HealthMonitoringContext _dbcontext;
          
        public ActivityDataRepository(HealthMonitoringContext dbcontext) :base(dbcontext) 
        {
           
           
        }
    }
}
