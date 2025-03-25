using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.Models.AIModels;

namespace HealthMonitoring.BLL.IServices
{
    public interface ILocalAIModelService
    {
        public  Task<BloodPressurePrediction> PredictBloodPressure(List<float> ecgValues, List<float> ppgValues);
    }
}
