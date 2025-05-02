using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HealthMonitoring.BLL.IServices;
using HealthMonitoring.BLL.StaticData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Vonage;
using Vonage.Messaging;
using Vonage.Request;

namespace HealthMonitoring.BLL.Services
{
    public class SMSService : ISMSService
    {

        private readonly TwilioSettings _twiliosettings;
        private readonly IConfiguration _configuration;

        public SMSService(IOptions<TwilioSettings> options, IConfiguration configuration)
        {
            _twiliosettings = options.Value;
            _configuration = configuration;
        }
        public MessageResource SendMessage(string phonenumber, string body)
        {
            TwilioClient.Init(_twiliosettings.AccountSID, _twiliosettings.AuthToken);
            var result = MessageResource.Create(
                body: body,
                from: new Twilio.Types.PhoneNumber(_twiliosettings.TwilioPhoneNumber),
                to: phonenumber
                );
            return result;

        }

        public async Task SendSmsAsync(string toPhoneNumber, string message)
        {
            var credentials = Credentials.FromApiKeyAndSecret(
                _configuration["Vonage:ApiKey"],
                _configuration["Vonage:ApiSecret"]);

            var client = new SmsClient(credentials);

            var request = new SendSmsRequest
            {
                To = toPhoneNumber,
                From = _configuration["Vonage:From"],
                Text = message
            };

            var response = await client.SendAnSmsAsync(request);
            if (response.Messages[0].Status != "0")
            {
                throw new VonageSmsResponseException("0");
            }
        }
    

        }
}
