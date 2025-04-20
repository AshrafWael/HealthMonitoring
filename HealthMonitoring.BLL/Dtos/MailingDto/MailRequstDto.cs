using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HealthMonitoring.BLL.Dtos.MailingDto
{
    public class MailRequstDto
    {
        [Required]
        public string ToEmail { get; set; }
        [Required]

        public string Subject { get; set; }
        [Required]

        public string Body { get; set; }
        public  IList<IFormFile>? attachments { get; set; }

    }
}
