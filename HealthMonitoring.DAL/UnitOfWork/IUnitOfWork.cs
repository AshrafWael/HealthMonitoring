using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.Models;
using HealthMonitoring.DAL.IRepository;
using HealthMonitoring.DAL.IRepository.IAIRepository;

namespace HealthMonitoring.DAL.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        //Refrance for all repos 
        IActivityDataRepository ActivityDatas { get; }
        ICaloriesPredictionRepository CaloriesPredictions { get; }
        IHeartRateDataRepository HeartRateDatas { get; }
        IHeartDiseaseRepository HeartDiseases { get; }
        IBloodPressureReadingRepository bloodPressureReading {  get; }
        ISensorDataSetRepository sensorDataSet {  get; }
        IUserRepository Users { get; }
        IEmergancyContactReppository EmergancyContacts { get; }
        Task<int> SaveChangesAsync();
    }
}
