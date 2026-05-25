using document_sharing_manager.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace document_sharing_manager.Infrastructure.Persistence.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Action)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.EntityType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.EntityId)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.IpAddress)
                .HasMaxLength(50);

            builder.HasIndex(x => x.Action);
            builder.HasIndex(x => x.EntityType);
            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
