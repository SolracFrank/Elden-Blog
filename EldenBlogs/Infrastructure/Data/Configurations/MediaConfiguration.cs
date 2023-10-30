using Domain.Entities;
using Infrastructure.CustomEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class MediaConfiguration : IEntityTypeConfiguration<Media>
    {
        public void Configure(EntityTypeBuilder<Media> builder)
        {
            builder.ToTable(nameof(Media));

            builder.HasKey(b => b.Id_Media);

            builder.Property(b => b.Id_Media)
                .IsRequired()
                .HasColumnName("id_media")
                .ValueGeneratedOnAdd();

            builder.Property(b => b.MediaType)
                .IsRequired()
               .HasConversion<string>()
               .HasColumnName("media_type");

            builder.Property(b => b.MediaPath)
                .IsRequired()
               .HasColumnName("media_path");

            builder.Property(b => b.CreatedAt)
                .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(m => m.Fk_Id_User)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_media_user");
        }
    }
}
