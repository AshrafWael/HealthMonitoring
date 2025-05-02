using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HealthMonitoring.BLL.IServices
{
    public interface IMailingService
    {
        Task SendEmailAsync(string mailto,string Subject ,string Body,IList<IFormFile> attachments = null);
    }
}
