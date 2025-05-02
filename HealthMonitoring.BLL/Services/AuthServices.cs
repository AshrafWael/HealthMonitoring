
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using HealthMonitoring.BLL.Dtos.AccountUserDtos;
using HealthMonitoring.BLL.Dtos.ApplicationUserDtos;
using HealthMonitoring.BLL.IServices;
using HealthMonitoring.DAL.Data.Models;
using HealthMonitoring.DAL.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Azure;
using System.Net;
using HealthMonitoring.BLL.Dtos.ApplicationUserDtos.AccountUserDtos;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.BLL.Services
{
    public class AuthServices :IAuthServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMailingService _mailingService;
        private readonly IMapper _mapper;
        private string secretkey;

        public AuthServices( IUnitOfWork unitOfWork,IConfiguration configuration,
            UserManager<ApplicationUser> userManager, IMapper mapper
            , RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> SignInManager,
            IMailingService mailingService)
        {
            
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
            _signInManager = SignInManager;
            _mailingService = mailingService;
            secretkey = configuration.GetValue<string>("ApiSettings:SecretKey")!;
        }
        public async Task<bool> IsUniqueUser(string username)
        {
            var user = await _unitOfWork.Users.FindUserAsync(u => u.UserName == username);
            if (user == null)
            {
                return true;
            }
            return false;
        }
        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {

            var user = await _unitOfWork.Users.FindUserAsync(u =>
               u.Email == loginRequestDto.Email || u.UserName.ToUpper() == loginRequestDto.Username.ToUpper());
            
            var email = _userManager.GetEmailAsync(user);

           
            if (user == null)
            {
                return new LoginResponseDto()
                {
                    Token = "",
                    User = null
                };
            }
      
            if (user.EmailConfirmed == false)
            {
                return new LoginResponseDto()
                {
                    Token = "",
                    User = null
                    
                };
            }
            bool IsValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if ( IsValid == false)
            {
                return new LoginResponseDto()
                {
                    Token = "",
                    User = null
                };

            }
            //generate token 

            var Roles = await _userManager.GetRolesAsync(user);
            var tokenhandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretkey);

            var tokendescreptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
               {
                new Claim(ClaimTypes.Name,user.Email.ToString()),
                new Claim(ClaimTypes.Role,Roles.FirstOrDefault())

               }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenhandler.CreateToken(tokendescreptor);
            LoginResponseDto loginResponse = new LoginResponseDto()
            {

                Token = tokenhandler.WriteToken(token),
                User = _mapper.Map<ApplicationUserReadDto>(user),
                // Role = Roles.FirstOrDefault(),
            };
            return loginResponse;

        }
        public async Task<ApplicationUserReadDto> Register(RegisterRequestDto registerRequestDto)
        {
            ApplicationUser user = new()
            {
                Email = registerRequestDto.Email,
                NormalizedEmail = registerRequestDto.Email.ToUpper(),
                UserName = registerRequestDto.UserName,

            };
            try
            {
                try
                {
                    // Check if roles exist, create them if not
                    if (!await _roleManager.RoleExistsAsync("Admin"))
                        await _roleManager.CreateAsync(new IdentityRole("Admin"));

                    if (!await _roleManager.RoleExistsAsync("User"))
                        await _roleManager.CreateAsync(new IdentityRole("User"));

                    if (!await _roleManager.RoleExistsAsync("Contributor"))
                        await _roleManager.CreateAsync(new IdentityRole("Contributor"));
                 
                    // Create the user
                    var result = await _userManager.CreateAsync(user, registerRequestDto.Password);
                    if (result.Succeeded)
                    {
                        // Check if this is the first user
                        var allUsers = await _userManager.Users.ToListAsync();
                        if (allUsers.Count == 1)
                        {
                            await _userManager.AddToRoleAsync(user, "Admin");
                        }
                        else
                        {
                            // Respect the requested role, default to "User"
                            var roleToAssign = registerRequestDto.Role.ToString();

                            await _userManager.AddToRoleAsync(user, roleToAssign);
                        }
                         await ConfirmEmailAsync(user);
                        var usertoreturn = await _unitOfWork.Users
                        .FindUserAsync(u => u.Email == registerRequestDto.Email);
                        return _mapper.Map<ApplicationUserReadDto>(usertoreturn);
                    }
                }
                catch (Exception ex)
                { }
                return new ApplicationUserReadDto();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while registering user", ex);
            }
        }
        public async Task UpdateUser(ApplicationUserUpdateDto userUpdateDto,string id ) 
        {
               var user =await   _unitOfWork.Users.FindUserAsync(u => u.Id == userUpdateDto.ID);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found.");

            }
            var mappeduser = _mapper.Map(userUpdateDto, user);
            await  _unitOfWork.Users.UpdateAsync(mappeduser);
                _unitOfWork.SaveChanges();
        }
        public async Task<string> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return "Signed Out";
            }
            catch (Exception ex)
            {
                throw new Exception("Logout failed", ex);
            }
        }
        public async Task<bool> ConfirmEmailAsync(ApplicationUser user)
        {
            var emailAddress = user.Email;
            var Subject = "Email Confirmation";
            var ConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.HtmlEncode(ConfirmationToken); // should be encoded because it has special characters
            var confirmationLink = $"{_configuration["AppUrl"]}/api/Users/ConfirmEmail?userId={user.Id}&token={encodedToken}";
            var body = GetEmailTemplate(confirmationLink);
            await _mailingService.SendEmailAsync(emailAddress, Subject, body);
            return true;
        }
        private string GetEmailTemplate(string confirmationLink)
        {
            return @$"
            <html>
            <body style='font-family: Arial, sans-serif; margin: 0; padding: 0;'>
                <div style='max-width: 600px; margin: 20px auto; background: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                    <h2 style='color: #333; text-align: center;'>Welcome!</h2>
                    <p style='color: #666; text-align: center;'>Please confirm your email address to complete your registration.</p>
                    <div style='text-align: center; margin: 30px 0;'>
                      
                        <a href='{confirmationLink}' 
                        style='background-color: #4CAF50; 
                                color: white; 
                                padding: 15px 30px; 
                                text-decoration: none; 
                                border-radius: 5px; 
                                font-weight: bold;
                                display: inline-block;'>
                            Confirm Email
                        </a>
                    </div>
                </div>
            </body>
            </html>";
        }
        public async Task<bool> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error.Description);
                }
            }
            return result.Succeeded;
        }
        public async Task<List<ApplicationUserReadDto>> GetAllUsers()
        {
         var users = await   _unitOfWork.Users.GetAllAsync();
            return _mapper.Map<List<ApplicationUserReadDto>>(users);
        }
        public async Task<ApplicationUserReadDto> GetUserByName(string username)
        {
            var user = await _unitOfWork.Users.FindAsync( u => u.UserName.ToLower() == username.ToLower());
            if (user == null)
            {
                throw new KeyNotFoundException($"User with username {username} not found.");
            }
            var mappeduser = _mapper.Map<ApplicationUserReadDto>(user);
            return mappeduser;
        }
        public async Task<ApplicationUserReadDto> GetUserById(string userid)
        {
            var user = await _unitOfWork.Users.FindAsync(u => u.Id == userid);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with userid {userid} not found.");
            }
            var mappeduser = _mapper.Map<ApplicationUserReadDto>(user);
            return mappeduser;
        }
        public async Task<bool> DeletUser(string userid)
        {
            var user = await _unitOfWork.Users.FindAsync(u => u.Id == userid);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with userid {userid} not found.");
            }
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                throw new Exception("Failed to delete user");
            }
        }
        public async Task<string> ResetPassword(ResetPasswordDto ResetPasswordData)
        {
            var user = await _userManager.FindByEmailAsync(ResetPasswordData.email);
            if (user == null)
            {
                return "Email Is Wrong";
            }
            if (!await _userManager.CheckPasswordAsync(user, ResetPasswordData.CurrentPassword))
            {
                return "Wrong Passsword";
            }
            if (ResetPasswordData.CurrentPassword == ResetPasswordData.NewPassword)
            {
                return "The New Password Can not Be the Same As Current Password";
            }
            var change = await _userManager.ChangePasswordAsync
                (user, ResetPasswordData.CurrentPassword, ResetPasswordData.NewPassword);
            return "Password Changed Correctly";
        }
        public async Task<bool> ResetForgotPasswordAsync(ForgotPasswordDto resetDto)
        {
            var user = await _userManager.FindByIdAsync(resetDto.UserId);
            if (user == null)
                return false;

            var result = await _userManager.ResetPasswordAsync(user, resetDto.Token, resetDto.NewPassword);
            return result.Succeeded;
        }
        public async Task<bool> SendPasswordResetEmailAsync(ApplicationUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            
            var resetLink = $"{_configuration["AppUrl"]}/api/Users/ResetForgotPassword?userId={user.Id}&token={encodedToken}";
           // var decodedToken = WebUtility.UrlDecode(encodedToken);
            var body = GetResetPasswordEmailTemplate(resetLink);

            await _mailingService.SendEmailAsync(user.Email, "Password Reset", body);
            return true;
        }
        public async Task<ApplicationUser> FindByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return null;
            }
            return user;
        }
        private string GetResetPasswordEmailTemplate(string resetLink)
        {
            return @$"
    <html>
    <body style='font-family: Arial, sans-serif;'>
        <div style='max-width: 600px; margin: 20px auto; background: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
            <h2 style='color: #333; text-align: center;'>Reset Your Password</h2>
            <p style='color: #666; text-align: center;'>Click the button below to reset your password:</p>
            <div style='text-align: center; margin: 30px 0;'>
                <a href='{resetLink}' 
                   style='background-color: #007BFF; 
                          color: white; 
                          padding: 15px 30px; 
                          text-decoration: none; 
                          border-radius: 5px; 
                          font-weight: bold;
                          display: inline-block;'>Reset Password</a>
            </div>
        </div>
    </body>
    </html>";
        }


    }
}
