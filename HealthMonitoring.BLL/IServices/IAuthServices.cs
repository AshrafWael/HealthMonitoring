using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.BLL.Dtos.AccountUserDtos;
using HealthMonitoring.BLL.Dtos.ApplicationUserDtos;

namespace HealthMonitoring.BLL.IServices
{
    public interface IAuthServices
    {
        public Task<bool> IsUniqueUser(string username);
        public Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        public Task<ApplicationUserReadDto> Register(RegisterRequestDto registerRequestDto);
        public Task UpdateUser(ApplicationUserUpdateDto userUpdateDto,string id);
    }
}
