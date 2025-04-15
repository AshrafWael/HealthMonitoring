using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.BLL.Dtos.ActivityDataDtos
{
    public class ActivityDataUpdateDto
    {
        public DateTime RecordedAt { get; set; }
        public string ActivityType { get; set; } //Running, Sleeping ,Waking
        public float Duration { get; set; }  //in Hours
        public float Distance { get; set; }
        public int CaloriesBurned { get; set; }
        public string SleepQuality { get; set; } // e.g., "Good", "Fair", "Poor"
        public string UserId { get; set; }

    }
}
