using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HealthMonitoring.API.Swagger
{
    public class CustomSwaggerOrder : IDocumentFilter
    {
        private readonly List<string> _controllerOrder = new()
    {
        "Users",
        "Admin",
        "EmergencyContact",
        "SensorData"
    };

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var orderedPaths = swaggerDoc.Paths
                .OrderBy(p =>
                {
                    // Extract controller name from the route — assuming /api/{Controller}/...
                    var segments = p.Key.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    var controllerName = segments.Length > 1 ? segments[1] : segments.FirstOrDefault() ?? "";

                    var index = _controllerOrder.IndexOf(controllerName);
                    return index >= 0 ? index : int.MaxValue; // Controllers not in list go last
                })
                .ToList();

            swaggerDoc.Paths = new OpenApiPaths();
            foreach (var (key, value) in orderedPaths)
            {
                swaggerDoc.Paths.Add(key, value);
            }
        }

        private int GetOrderIndex(string path)
        {
            foreach (var (name, index) in _controllerOrder.Select((value, i) => (value.ToLower(), i)))
            {
                if (path.ToLower().Contains($"/{name.ToLower()}"))
                    return index;
            }

            return int.MaxValue; // Anything not listed comes last
        }


//        in my asp.net core web api i have those to etnity
//   public class EmergencyContact
//        {
//            [Key]
//            public int ContactId { get; set; }
//            public string Name { get; set; }
//            public string PhoneNumber { get; set; }
//            public string Email { get; set; }

//            public ICollection<ApplicationUser> ApplicationUsers { get; set; } = new List<ApplicationUser>();
//        }
//        public class ApplicationUser : IdentityUser
//        {
//            public string? Name { get; set; }
//            public string? FirstName { get; set; }
//            public string? LastName { get; set; }
//            public string? Address { get; set; }
//            public DateTime? DateOfBirth { get; set; }
//            public string? Gender { get; set; }
//            public int? Age { get; set; }
//            public int? Weight { get; set; }
//            public int? Height { get; set; }
//            public string? HealthGoals { get; set; }
//            public DateTime CreatedDate { get; set; } = DateTime.Now;
//            public DateTime UpdatedAt { get; set; } = DateTime.Now;
//            //Navigation Proprty
//            public ICollection<EmergencyContact> EmergencyContacts { get; set; } = new List<EmergencyContact>();
//            public ICollection<ActivityData> activityDatas { get; set; }
//            public ICollection<HeartDisease> HealthInformation { get; set; }
//            public ICollection<HeartRateData> HeartRateDatas { get; set; }
//            public ICollection<BloodPressureReading> bloodPressureReadings { get; set; }
//        }
//        have an many to many relation between each other the efcore create a thired table called ApplicationUserEmergencyContact have an ApplicationUsersId and EmergencyContactsContactId
//so how can i get data from two table
//i am working on n-tair app so i want to add emergancy contact by user id and i want user can get all emergancy contact linked to hime by email  and name  and get them all
//and emegancy contact can connect with the user by user email and can get all user connected to him
//then make end point for all that
    }
}
