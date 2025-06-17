using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HealthMonitoring.DAL.Data.Models
{
    public class ActivityData
    {
        [Key]
        public int Id { get; set; }
        public DateTime RecordedAt { get; set; }
        public  string? ActivityType { get; set; } //Running, Sleeping ,Waking
        public float? Duration { get; set; }  //in Hours
      //  public float Distance { get; set; }
        public int? CaloriesBurned { get; set; }
        public string? SleepQuality { get; set; } // e.g., "Good", "Fair", "Poor"

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
