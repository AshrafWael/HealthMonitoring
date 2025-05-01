using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.BLL.Dtos.HealthInformationDtos
{
    public class HeartDiseasesReadingDto
    {
        public string Diseases { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
