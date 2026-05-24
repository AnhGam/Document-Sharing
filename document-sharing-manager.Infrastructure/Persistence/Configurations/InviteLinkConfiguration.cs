using document_sharing_manager.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace document_sharing_manager.Infrastructure.Persistence.Configurations
{
    public class InviteLinkConfiguration : IEntityTypeConfiguration<InviteLink>
    {
        public void Configure(EntityTypeBuilder<InviteLink> builder)
        {
            builder.ToTable("InviteLinks");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(x => x.Code).IsUnique();
        }
    }
}
