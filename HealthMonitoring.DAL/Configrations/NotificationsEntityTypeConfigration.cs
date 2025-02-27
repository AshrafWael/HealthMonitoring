using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthMonitoring.DAL.Configrations
{
    public class NotificationsEntityTypeConfigration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasOne(u => u.User)
                  .WithMany(H => H.Notifications)
                  .HasForeignKey(u => u.UserId);

            builder.HasOne(u => u.Contact)
                  .WithMany(H => H.notifications)
                  .HasForeignKey(u => u.ContactId);


            builder.Property(x => x.Message)
          .IsRequired()
          .HasMaxLength(500);

            builder.Property(x => x.NotificationType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Type)
                .HasMaxLength(50);
        }
    }
}
