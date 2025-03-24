using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.BLL.Dtos.AIModelDtos
{
    public class SensorDataRequestDto
    {
        public List<double> ECG { get; set; } = new List<double>();
        public List<double> ABP { get; set; } = new List<double>();
        public List<double> PPG { get; set; } = new List<double>();
    }
}
