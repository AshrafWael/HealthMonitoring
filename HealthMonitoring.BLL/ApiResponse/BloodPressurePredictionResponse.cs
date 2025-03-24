using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.API.ApiResponse;
using HealthMonitoring.DAL.Data.Models.AIModels;

namespace HealthMonitoring.BLL.ApiResponse
{
    public class BloodPressurePredictionResponse :APIResponse
    {
        public BloodPressurePrediction Prediction { get; set; }

    }
}
