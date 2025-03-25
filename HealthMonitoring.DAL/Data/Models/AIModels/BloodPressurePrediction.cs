using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HealthMonitoring.DAL.Data.Models.AIModels
{
    public class BloodPressurePrediction
    {
        // AI model prediction response
        public List<double> sbp { get; set; }
        public List<double> dbp { get; set; }


        [JsonIgnore]
        public BloodPressurePrediction Prediction => new BloodPressurePrediction
        {
            sbp = this.sbp,
            dbp = this.dbp
        };
    }
}
