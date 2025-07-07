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
        public int Day { get; set; } // Day number (0-30)
        public double TotalSteps { get; set; }
        public double VeryActiveMinutes { get; set; }
        public double FairlyActiveMinutes { get; set; }
        public double LightlyActiveMinutes { get; set; }
        public double SedentaryMinutes { get; set; }
        public double TotalMinutesAsleep { get; set; }
        public double WeightKg { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
