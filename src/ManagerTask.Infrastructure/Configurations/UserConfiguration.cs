using ManagerTask.Domain.Entities.UserEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManagerTask.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        
        builder.HasKey(x => x.Id).HasName("pk_users_id");
        builder.Property(x => x.Id).HasColumnName("id");
        
        builder.Property(x => x.ChatId)
            .IsRequired()
            .HasColumnName("chat_id");
        
        builder.HasIndex(x => x.ChatId)
            .IsUnique();
    }
}