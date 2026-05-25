using document_sharing_manager.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace document_sharing_manager.Infrastructure.Persistence.Configurations
{
    public class JoinRequestConfiguration : IEntityTypeConfiguration<JoinRequest>
    {
        public void Configure(EntityTypeBuilder<JoinRequest> builder)
        {
            builder.ToTable("JoinRequests");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DisplayName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.InviteCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(x => x.Status);
            
            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
