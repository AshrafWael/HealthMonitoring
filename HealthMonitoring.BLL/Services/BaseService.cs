using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.API.ApiResponse;
using HealthMonitoring.BLL.APIRequst;
using HealthMonitoring.BLL.IServices;
using static HealthMonitoring.BLL.StaticData.StaticData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

namespace HealthMonitoring.BLL.Services
{
    public class BaseService :IBaseSrvice
    {
        public IHttpClientFactory _httpClient { get; set; }
        public BaseService(IHttpClientFactory httpClient)
        {
              ResponceModel = new();
            _httpClient = httpClient;
        }
        public APIResponse ResponceModel
        {
            get;
            set;
        }
        
                public async Task<T> SendAsync<T>(APIRequest apirequest)
                {
                    try
                    {

                            var Client = _httpClient.CreateClient("HealthMonitoring.API");
                        HttpRequestMessage message = new HttpRequestMessage();
                        //heder type 
                        message.Headers.Add("Accept", "application/json");
                        message.RequestUri = new Uri(apirequest.ApiUrl);
                        if (apirequest.Data != null)
                        {
                            message.Content = new StringContent(JsonConvert.SerializeObject(apirequest.Data)
                                , Encoding.UTF8, "application/json");
                        }
                        switch (apirequest.ApiType)
                        {
                            case ApiType.POST:
                                message.Method = HttpMethod.Post;
                                break;
                            case ApiType.PUT:
                                message.Method = HttpMethod.Put;
                                break;
                            case ApiType.DELETE:
                                message.Method = HttpMethod.Delete;
                                break;
                            default:
                                message.Method = HttpMethod.Get;
                                break;
                        }
                        HttpResponseMessage apiResponse = null;
                        //call our client
                        // Send request with cancellation support
                        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30)); // Add timeout

                        apiResponse = await Client.SendAsync(message);
                        var apicontent = await apiResponse.Content.ReadAsStringAsync();
                        try
                        {
                            APIResponse APiResponse = JsonConvert.DeserializeObject<APIResponse>(apicontent);
                            if (apiResponse.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                                apiResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                            {
                                APiResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                                APiResponse.IsSuccess = false;

                                var result = JsonConvert.SerializeObject(APiResponse);
                                var returnObj = JsonConvert.DeserializeObject<T>(result);
                                return returnObj;

                            }
                        }
                        catch (Exception ex)
                        {
                            var exptionResponse = JsonConvert.DeserializeObject<T>(apicontent);
                            return exptionResponse;
                        }
                        var ApiResponse = JsonConvert.DeserializeObject<T>(apicontent);
                        return ApiResponse;

                    }
                    catch (Exception ex)
                    {
                        var dto = new APIResponse
                        {
                            Errors = new List<string>()
                            {
                                Convert.ToString(ex.Message)
                            },
                            IsSuccess = false,
                        };
                        var result = JsonConvert.SerializeObject(dto);
                        var ApiResponse = JsonConvert.DeserializeObject<T>(result);
                        return ApiResponse;

                    }
                }
        
        
             

    }
}
