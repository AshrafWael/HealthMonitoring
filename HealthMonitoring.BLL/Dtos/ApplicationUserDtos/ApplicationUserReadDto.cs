using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.BLL.Dtos.ApplicationUserDtos
{
    public class ApplicationUserReadDto
    {
        public  string ID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? Role { get; set; } = "User";
    }
}
