
using HealthMonitoring.API.ApiResponse;
using HealthMonitoring.BLL.APIRequst;

namespace HealthMonitoring.BLL.IServices
{
    public interface IBaseSrvice
    {
        APIResponse ResponceModel { get; set; }
        Task<T> SendAsync<T>(APIRequest apirequest);  
          
    }
}
