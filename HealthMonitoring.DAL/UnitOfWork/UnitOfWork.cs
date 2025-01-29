using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.DbHelper;
using HealthMonitoring.DAL.Data.Models;
using HealthMonitoring.DAL.IRepository;
using HealthMonitoring.DAL.Repository;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
       private readonly HealthMonitoringContext _dbcontext;
       public  IActivityDataRepository  ActivityDatas { get; private set; }
       public IBaseRepository<EmergencyContact> EmergencyContacts { get; private set; }
        public UnitOfWork(HealthMonitoringContext dbcontext)
        {
            _dbcontext = dbcontext;
            ActivityDatas = new ActivityDataRepository(_dbcontext);
            EmergencyContacts = new BaseRepository<EmergencyContact>(_dbcontext);
        }
        public int SaveChanges()
        { 
          return _dbcontext.SaveChanges();
        }    
        public void Dispose()
        {
           _dbcontext.Dispose();
        }

    }
}
