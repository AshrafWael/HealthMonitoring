using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.DAL.Data.Models
{
    public class CaloriesPrediction
    {
        public int Id { get; set; }
        public double PredictedCalories { get; set; }
        public DateTime PredictionDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ActivityDtataId { get; set; } 
        // Foreign key to ActivityData
        public ActivityData ActivityData { get; set; }
        // Foreign key to ApplicationUser
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
