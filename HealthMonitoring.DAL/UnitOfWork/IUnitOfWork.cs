using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.Models;
using HealthMonitoring.DAL.IRepository;

namespace HealthMonitoring.DAL.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        //Refrance for all repos 
        IActivityDataRepository ActivityDatas { get; }
        IBaseRepository<EmergencyContact> EmergencyContacts { get; }
        int SaveChanges ();
    }
}
