
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

namespace HealthMonitoring.BLL.Services
{
    public class AuthServices :IAuthServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private string secretkey;

        public AuthServices( IUnitOfWork unitOfWork,IConfiguration configuration,
            UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
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
            bool IsValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (user == null || IsValid == false)
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
                var result = await _userManager.CreateAsync(user, registerRequestDto.Password);
                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole("admin"));
                    }
                    await _userManager.AddToRoleAsync(user, "admin");
                    var usertoreturn = await _unitOfWork.Users.FindUserAsync(u =>
                    u.Email == registerRequestDto.Email);
                    return _mapper.Map<ApplicationUserReadDto>(usertoreturn);
                }
            }
            catch (Exception ex)
            { }
            return new ApplicationUserReadDto();

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
    }
}
