using Domain.Entities;
using Infrastructure.CustomEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class BlogConfiguration : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.ToTable(nameof(Blog));

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasColumnName("id_blog");

            builder.Property(b => b.BannerPath)
                .IsRequired(false)
                .HasColumnName("blog_banner")
                .HasMaxLength(255);

            builder.Property(b => b.BlogTitle)
                .IsRequired()
                .HasColumnName("blog_title")
                .HasMaxLength(50);

            builder.Property(b => b.BlogDescription)
                .IsRequired(false)
                .HasColumnName("blog_description")
                .HasMaxLength(255);

            builder.Property(b => b.CreatedAt)
                .IsRequired();

            builder.Property(b => b.UpdatedAt)
                .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(b => b.Fk_Id_User)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_blog_user");

        }
    }
}
