using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.BLL.Dtos.AIModelDtos
{
    public class BloodPressureReadingDto
    {
        public int Id { get; set; }
        public List<double> sbp { get; set; }
        public List<double> dbp { get; set; }
 
    }
}
