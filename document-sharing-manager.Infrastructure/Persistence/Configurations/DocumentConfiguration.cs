using document_sharing_manager.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace document_sharing_manager.Infrastructure.Persistence.Configurations
{
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.ToTable("Documents");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Ten)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(d => d.DinhDang)
                .HasMaxLength(50);

            builder.Property(d => d.DuongDan)
                .IsRequired();

            builder.Property(d => d.GhiChu)
                .HasMaxLength(1000);

            builder.Property(d => d.Tags)
                .HasMaxLength(500);

            builder.Property(d => d.KichThuoc)
                .HasColumnType("decimal(18,2)");

            // Global Query Filter for Soft Delete
            builder.HasQueryFilter(d => !d.IsDeleted);

            // Indexing for performance
            builder.HasIndex(d => d.Ten);
            builder.HasIndex(d => d.IsDeleted);
        }
    }
}
