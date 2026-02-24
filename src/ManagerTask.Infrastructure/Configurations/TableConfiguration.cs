using ManagerTask.Domain.Entities.TableEntity;
using ManagerTask.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManagerTask.Infrastructure.Configurations;

public class TableConfiguration : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
        builder.ToTable("tables");

        builder.HasKey(t => t.Id).HasName("pk_tables");
        builder.Property(t => t.Id).HasColumnName("id");

        builder.Property(t => t.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(Constants.Lenght128);

        builder.Property(t => t.Description)
            .HasColumnName("description")
            .HasMaxLength(Constants.Lenght1024);

        builder.HasMany(t => t.Tasks)
            .WithOne(t => t.Table)
            .HasForeignKey("table_id");

        builder.HasIndex(t => t.Name)
        .IsUnique();
    }
}