using DevLog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevLog.Infrastructure.Configurations;

public sealed class LogTagsConfiguration : IEntityTypeConfiguration<LogTags>
{
    public void Configure(EntityTypeBuilder<LogTags> builder)
    {
        builder.HasKey(x => new { x.LogId, x.TagId });

        builder.HasOne(x => x.Log)
            .WithMany(x => x.LogTags)
            .HasForeignKey(x => x.LogId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Tag)
            .WithMany(x => x.LogTags)
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}