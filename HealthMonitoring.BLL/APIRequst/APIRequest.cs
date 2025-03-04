using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using static HealthMonitoring.BLL.StaticData.StaticData;

namespace HealthMonitoring.BLL.APIRequst
{
    public class APIRequest
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string ApiUrl { get; set; }
        public object Data { get; set; }
    }
}
