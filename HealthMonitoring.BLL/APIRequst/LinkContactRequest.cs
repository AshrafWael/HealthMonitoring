using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.BLL.APIRequst
{
    public class LinkContactRequest
    {
        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }

        [Required]
        public int ContactId { get; set; }
    }
}
