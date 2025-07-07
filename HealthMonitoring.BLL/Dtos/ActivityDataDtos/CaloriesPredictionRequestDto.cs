using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.BLL.Dtos.ActivityDataDtos
{
    public class CaloriesPredictionRequestDto
    {
        public string UserId { get; set; }
        public int Day { get; set; } 
    }
}
