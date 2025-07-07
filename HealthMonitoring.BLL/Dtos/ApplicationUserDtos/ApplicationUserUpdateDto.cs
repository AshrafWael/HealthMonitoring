using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.BLL.Dtos.ApplicationUserDtos
{
    public class ApplicationUserUpdateDto
    {
        public string ID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public int? Age { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public double WeightKg { get; set; }
        public int Height { get; set; }
        public string? HealthGoals { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
