using ManagerTask.Domain.Entities.TaskEntity;
using ManagerTask.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task = ManagerTask.Domain.Entities.TaskEntity.Task;

namespace ManagerTask.Infrastructure.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<Task>
{

    public void Configure(EntityTypeBuilder<Task> builder)
    {
        builder.ToTable("tasks");

        builder.HasKey(t => t.Id).HasName("pk_tasks");
        builder.Property(t => t.Id).HasColumnName("id");

        builder.Property(t => t.Name)
            .HasColumnName("name")
            .HasMaxLength(Constants.Lenght128)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasColumnName("description")
            .HasMaxLength(Constants.Lenght1024);

        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(t => t.SendTime)
            .HasColumnName("send_time")
            .IsRequired();

        builder.Property(t => t.Status)
            .HasConversion(s => (int)s, s => (StatusTask)s)
            .HasColumnName("status")
            .IsRequired();

        builder.HasOne(t => t.Table)
            .WithMany(t => t.Tasks)
            .HasForeignKey("table_id");

        builder.HasIndex(t => t.Name)
        .IsUnique();
    }
}