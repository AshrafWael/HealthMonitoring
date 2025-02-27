using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.DbHelper;
using HealthMonitoring.DAL.Data.Models;
using HealthMonitoring.DAL.IRepository;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HealthMonitoring.DAL.Repository
{
    public class NotificationRepository :BaseRepository<Notification> ,INotificationRepository
    {
        private readonly HealthMonitoringContext _context;
        internal DbSet<Notification> _dbset;    
        public NotificationRepository(HealthMonitoringContext context) : base(context) 
        {
            _context = context;
            _dbset = _context.Set<Notification>();
        }

        public async Task<IEnumerable<Notification>> GetNotificationsByTypeAsync(string userId, string type)
        {
            IQueryable<Notification> query = _dbset;
            return await query.Where(n => n.UserId == userId && n.NotificationType == type)
                .OrderByDescending(n => n.SentAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId)
        {
            IQueryable<Notification> query = _dbset;
            return await query
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.SentAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId)
        {
            IQueryable<Notification> query = _dbset;
            return await query
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.SentAt)
                .ToListAsync();
        }

        public async Task  MarkAsReadAsync(int notificationId)
        {
            /* IQueryable < Notification > query =_dbset*/
            // 
            var notification = await _dbset.FindAsync(notificationId);

            if (notification != null)
            {
                notification.IsRead = true;
               
            }
        }
    }
}
