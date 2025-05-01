using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.BLL.Dtos.HeartRateDataDtos
{
    public class HeartRateReadingDto
    {
        public DateTime RecordedAt { get; set; }
        public int HeartRate { get; set; }
        public string Category { get; set; }
    }
}
