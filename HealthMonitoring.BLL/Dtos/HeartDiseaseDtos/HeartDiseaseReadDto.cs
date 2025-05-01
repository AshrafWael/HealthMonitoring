using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HealthMonitoring.BLL.Dtos.HeartRateDataDtos;

namespace HealthMonitoring.BLL.Dtos.HealthInformationDtos
{
    public class HeartDiseaseReadDto
    {
        public string Prediction { get; set; }

        //[JsonIgnore]
        //public HeartDiseaseReadDto Disease => new HeartDiseaseReadDto
        //{
        //    Prediction = this.Prediction
        //};
    }
}
