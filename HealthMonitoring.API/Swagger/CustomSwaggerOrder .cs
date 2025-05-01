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
    }
}
