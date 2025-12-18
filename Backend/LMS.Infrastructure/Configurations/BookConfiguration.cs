using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasOne(b => b.User)
               .WithMany(u => u.Books)
               .HasForeignKey(b => b.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(b => !b.IsDeleted);

            builder.Property(b => b.ReadingStatus)
                   .HasConversion<string>();
        }
    }
}
