using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.BLL.Dtos.ApplicationUserDtos.AccountUserDtos
{
    public class LogoutResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public DateTime LogoutTime { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
