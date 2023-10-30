using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class BlogSettingsConfiguration : IEntityTypeConfiguration<BlogSetting>
    {
        public void Configure(EntityTypeBuilder<BlogSetting> builder)
        {
            builder.ToTable(nameof(BlogSetting));

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .IsRequired()
                .HasDefaultValueSql("newid()");

            builder.Property(b => b.IsMature)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasOne<Blog>()
                .WithOne()
                .HasForeignKey<BlogSetting>(b => b.Fk_Blog_Id)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_blog_blogsettings");

        }
    }
}
