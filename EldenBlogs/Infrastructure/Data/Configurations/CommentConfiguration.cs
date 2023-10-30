using Domain.Entities;
using Infrastructure.CustomEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable(nameof(Comment));

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasColumnName("id_comment");

            builder.Property(c => c.CommentContent)
                .IsRequired()
                .HasColumnName("content")
                .HasMaxLength(255);

            builder.Property(c => c.Likes)
                .IsRequired();

            builder.Property(c => c.Dislikes)
                .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(b => b.Fk_Id_User)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_comment_user");

            builder.HasOne<Entry>()
                .WithMany()
                .HasForeignKey(c => c.Fk_Id_Entrada)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_comment_entry");

        }
    }
}
