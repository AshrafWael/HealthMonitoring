using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.BLL.Dtos.AccountUserDtos;
using HealthMonitoring.BLL.Dtos.ApplicationUserDtos;
using HealthMonitoring.BLL.Dtos.ApplicationUserDtos.AccountUserDtos;
using HealthMonitoring.BLL.Dtos.MailingDto;
using HealthMonitoring.DAL.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HealthMonitoring.BLL.IServices
{
    public interface IAuthServices
    {
        public Task<bool> IsUniqueUser(string username);
        public Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        public Task<ApplicationUserReadDto> Register(RegisterRequestDto registerRequestDto);

        public Task<bool> ConfirmEmail(string userId, string token);
        public Task<bool> ConfirmEmailAsync(ApplicationUser user);
        public  Task<ApplicationUser> FindByEmail(string email);
        public Task UpdateUser(ApplicationUserUpdateDto userUpdateDto,string id);
        public  Task<string> Logout();
        public Task<string> ResetPassword(ResetPasswordDto ResetPasswordData);
        public Task<List<ApplicationUserReadDto>> GetAllUsers();
        public Task<ApplicationUserReadDto> GetUserByName(string username);
        public Task<ApplicationUserReadDto> GetUserById(string userid);
        public Task<bool> DeletUser(string userid);
        public Task<bool> ResetForgotPasswordAsync(ForgotPasswordDto resetDto);
        public  Task<bool> SendPasswordResetEmailAsync(ApplicationUser user);




    }
}
