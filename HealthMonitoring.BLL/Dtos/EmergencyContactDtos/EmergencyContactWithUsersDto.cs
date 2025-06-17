using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.BLL.Dtos.EmergencyContactDtos
{
    public class EmergencyContactWithUsersDto
    {
        public int ContactId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
         public List<string> ConnectedUsers { get; set; } = new List<string>();
    }
}
