using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.BLL.Dtos.AIModelDtos
{
    public class DataSetDto
    {
        public string UserId { get; set; }
        public List<double> PPG { get; set; }
        public List<double>? ABP { get; set; }
        public List<double> ECG { get; set; }
        public DateTime Timestamp { get; set; }

    }
}
