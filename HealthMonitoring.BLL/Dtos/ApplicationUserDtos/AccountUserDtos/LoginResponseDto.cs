using HealthMonitoring.BLL.Dtos.ApplicationUserDtos;

namespace HealthMonitoring.BLL.Dtos.AccountUserDtos
{
    public class LoginResponseDto
    {
       public  ApplicationUserReadDto User { get; set; }

        public string Token { get; set; }

    }
}
