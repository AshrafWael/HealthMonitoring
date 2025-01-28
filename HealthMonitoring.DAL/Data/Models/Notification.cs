using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.DAL.Data.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        public string Message { get; set; }
        public string NotificationType { get; set; } // e.g., Water Reminder
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; } // Whether the notification has been read

        public string Type { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }



        [ForeignKey("Contact")]
        public int ContactId { get; set; }
        public EmergencyContacts Contact { get; set; }

    }
}
