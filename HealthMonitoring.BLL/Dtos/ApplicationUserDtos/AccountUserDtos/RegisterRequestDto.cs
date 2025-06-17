namespace HealthMonitoring.BLL.Dtos.AccountUserDtos
{
    public class RegisterRequestDto
    {
        public enum UserRole
        {
            User,
            Contributor
        }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; } 

    }
}
