using Domain.Dtos.Token;
using Infrastructure.CustomEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(x => x.Id);

            builder.Property(b => b.Id)
                .IsRequired()
                .HasColumnName("Id_RefreshToken");

            builder.HasIndex(t => t.Token)
                .IsUnique();

            builder.Property(b => b.Token)
              .IsRequired();

            builder.Property(b => b.Expires)
              .IsRequired();

            builder.Property(b => b.RevokedByIp)
            .IsRequired(false);

            builder.Property(b => b.TokenReplaced)
            .IsRequired(false);

            builder.Property(b => b.Created)
            .IsRequired();

            builder.Property(b => b.RevokedByIp)
            .IsRequired(false);

            builder.Property(b => b.UserId)
              .IsRequired();

            builder.HasOne<User>()
              .WithMany()
              .HasForeignKey(u => u.UserId)
              .OnDelete(DeleteBehavior.Cascade)
              .HasConstraintName("FK_RefreshToken_ApplicationUser");
        }
    }
}
