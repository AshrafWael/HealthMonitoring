using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.BLL.StaticData;
using Twilio.Rest.Api.V2010.Account;

namespace HealthMonitoring.BLL.IServices
{
    public interface ISMSService
    {
        MessageResource SendMessage(string phonenumber, string body);
        public  Task SendSmsAsync(string toPhoneNumber, string message);
    }
}
