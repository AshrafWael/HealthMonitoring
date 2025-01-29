using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.DAL.Data.Models
{
    public class HealthSuggestion
    {
        //Stores AI-generated health suggestions
        //.Health recommendations and suggestions are based on manually entered data.
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Suggestion { get; set; } // Personalized health suggestion
        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
