using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.DbHelper;
using HealthMonitoring.DAL.Data.Models;
using HealthMonitoring.DAL.Data.Models.AIModels;
using HealthMonitoring.DAL.IRepository;
using HealthMonitoring.DAL.IRepository.IAIRepository;
using HealthMonitoring.DAL.Repository;
using HealthMonitoring.DAL.Repository.AIRepository;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
       private readonly HealthMonitoringContext _dbcontext;
       public  IActivityDataRepository  ActivityDatas { get; private set; }
        public IHeartRateDataRepository HeartRateDatas { get; private set; }
        public IHeartDiseaseRepository HeartDiseases { get; private set; }
        public IUserRepository Users { get; private set; }
       public IBaseRepository<EmergencyContact> EmergencyContacts { get; private set; }
        public IBloodPressureReadingRepository bloodPressureReading { get; private set; }
        public ISensorDataSetRepository sensorDataSet { get; private set; }

        public UnitOfWork(HealthMonitoringContext dbcontext)
        {
            _dbcontext = dbcontext;
            ActivityDatas = new ActivityDataRepository(_dbcontext);
            HeartRateDatas = new HeartRateDataRepository(_dbcontext);
            HeartDiseases = new HeartDiseaseRepository(_dbcontext);
            EmergencyContacts = new BaseRepository<EmergencyContact>(_dbcontext);
            bloodPressureReading = new BloodPressureReadingRepository(_dbcontext);
            sensorDataSet = new SensorDataSetRepository(_dbcontext);
            Users = new UserRepository(_dbcontext);
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
