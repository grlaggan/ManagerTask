using ManagerTask.Domain.Entities.NotificationEntity;
using ManagerTask.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManagerTask.Infrastructure.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");
        
        builder.HasKey(x => x.Id).HasName("pk_notifications_id");
        builder.Property(n => n.Id).HasColumnName("id");
        
        builder.Property(n => n.Name)
            .IsRequired()
            .HasMaxLength(Constants.Lenght128)
            .HasColumnName("name");
        
        builder.Property(n => n.Message)
            .HasMaxLength(Constants.Lenght1024)
            .HasColumnName("message");
        
        builder.Property(n => n.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");
        
        builder.Property(n => n.NotificationTime)
            .IsRequired()
            .HasColumnName("notification_time");

        builder.HasIndex(n => n.Name)
            .IsUnique();
    }
}