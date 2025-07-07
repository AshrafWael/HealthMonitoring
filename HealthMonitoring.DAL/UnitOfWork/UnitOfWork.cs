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
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HealthMonitoringContext _dbcontext;

        public IUserRepository Users { get; private set; }
        public IEmergancyContactReppository EmergancyContacts { get; private set; }
        public IActivityDataRepository ActivityDatas { get; private set; }
        public ICaloriesPredictionRepository CaloriesPredictions { get; private set; }
        public IHeartRateDataRepository HeartRateDatas { get; private set; }
        public IHeartDiseaseRepository HeartDiseases { get; private set; }
        public IBloodPressureReadingRepository bloodPressureReading { get; private set; }
        public ISensorDataSetRepository sensorDataSet { get; private set; }

        public UnitOfWork(HealthMonitoringContext dbcontext) // Add UserManager to constructor
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));

            ActivityDatas = new ActivityDataRepository(_dbcontext);
            CaloriesPredictions = new CaloriesPredictionRepository(_dbcontext);
            HeartRateDatas = new HeartRateDataRepository(_dbcontext);
            HeartDiseases = new HeartDiseaseRepository(_dbcontext);
            bloodPressureReading = new BloodPressureReadingRepository(_dbcontext);
            sensorDataSet = new SensorDataSetRepository(_dbcontext);
            Users = new UserRepository(_dbcontext);
            EmergancyContacts = new EmergancyContactReppository(_dbcontext); // Pass UserManager to repository
        }


        public async Task<int> SaveChangesAsync()
        {
            return await _dbcontext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbcontext.Dispose();
        }
    }
}
