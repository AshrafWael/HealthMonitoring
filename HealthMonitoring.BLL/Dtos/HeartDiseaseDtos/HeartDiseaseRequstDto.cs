using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.BLL.Dtos.HealthInformationDtos
{
    public class HeartDiseaseRequstDto
    {
        public string UserId { get; set; }
        public List<double> ECG { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
