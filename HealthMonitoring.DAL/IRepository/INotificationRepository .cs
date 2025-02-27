using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.Models;

namespace HealthMonitoring.DAL.IRepository
{
    public interface INotificationRepository
    {

        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId);
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId);
        Task MarkAsReadAsync(int notificationId);
        Task<IEnumerable<Notification>> GetNotificationsByTypeAsync(string userId, string type);

    }
}
