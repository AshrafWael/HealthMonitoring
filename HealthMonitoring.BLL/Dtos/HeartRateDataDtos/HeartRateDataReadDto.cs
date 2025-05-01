using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.Models.AIModels;

namespace HealthMonitoring.BLL.Dtos.HeartRateDataDtos
{
    public class HeartRateDataReadDto
    {
        public int HeartRate { get; set; }

        [JsonIgnore]
        public HeartRateDataReadDto Prediction => new HeartRateDataReadDto
        {
            HeartRate = this.HeartRate
        };
    }
}
