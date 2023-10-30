using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class EntryConfiguration : IEntityTypeConfiguration<Entry>
    {
        public void Configure(EntityTypeBuilder<Entry> builder)
        {
            builder.ToTable(nameof(Entry));

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasColumnName("id_entry");

            builder.Property(e => e.EntryTitle)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("title");

            builder.Property(e => e.EntryBody)
                .IsRequired(false)
                .HasColumnName("body");

            builder.Property(e => e.CreatedAt)
                .IsRequired();

            builder.Property(e => e.UpdatedAt)
                .IsRequired();

            builder.HasOne<Blog>()
                .WithMany()
                .HasForeignKey(b => b.Fk_Id_Blog)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_entry_blog");


        }
    }
}
