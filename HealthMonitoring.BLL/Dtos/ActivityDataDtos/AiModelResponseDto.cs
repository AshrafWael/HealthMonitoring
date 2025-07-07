using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.BLL.Dtos.ActivityDataDtos
{
    public class AiModelResponseDto
    {
        public double PredictedCalories { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
