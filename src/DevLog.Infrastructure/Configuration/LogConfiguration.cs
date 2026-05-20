using DevLog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DevLog.Infrastructure.Configuration;

public class LogConfiguration : IEntityTypeConfiguration<Log>
{
    public void Configure(EntityTypeBuilder<Log> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(l => l.Description)
               .IsRequired()
               .HasMaxLength(2000);

        builder.Property(l => l.Category)
               .IsRequired()
               .HasConversion<string>();

        builder.Property(l => l.Status)
               .IsRequired()
               .HasConversion<string>();

        builder.Property(l => l.UserId)
               .IsRequired();

        builder.Property(l => l.CreatedAt)
               .IsRequired();

        builder.Property(l => l.LastUpdatedAt)
               .IsRequired();

        // RELATIONSHIPS
        // Log belongs to one User
        // Already configured in UserConfiguration
        // No need to repeat — EF reads from both sides

        // INDEXES
        builder.HasIndex(l => l.UserId);

        builder.HasIndex(l => l.Category);

        builder.HasIndex(l => l.Status);

        builder.HasIndex(l => l.CreatedAt);

    }
}
