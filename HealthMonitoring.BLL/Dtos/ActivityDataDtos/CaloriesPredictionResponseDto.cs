using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.BLL.Dtos.ActivityDataDtos
{
    public class CaloriesPredictionResponseDto
    {
       // public string? UserId { get; set; }
        //public int Day { get; set; }
        public double PredictedCalories { get; set; }
        public DateTime PredictionDate { get; set; }
      //  public double UpdatedWeight { get; set; }
     //   public DailyActivityDataDto? ActivityData { get; set; }
    }
}
