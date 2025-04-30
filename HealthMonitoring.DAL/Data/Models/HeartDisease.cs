using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace HealthMonitoring.DAL.Data.Models
{
    public class HeartDisease
    {
        // Health information
        [Key]
        public int Id { get; set; }
        public string Diseases { get; set; } // Comma-separated list of diseases
        public DateTime RecordedAt { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
