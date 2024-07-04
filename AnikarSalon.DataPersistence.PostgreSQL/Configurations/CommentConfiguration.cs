using AnikarSalon.Persistence.Postgres.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnikarSalon.Persistence.Postgres.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<CommentEntity>
    {
        public void Configure(EntityTypeBuilder<CommentEntity> builder)
        {
            builder.HasKey(com => com.Id);

            builder
                .HasOne(com => com.Author)
                .WithMany(cli => cli.Comments)
                .HasForeignKey(com => com.AuthorId);

            builder
                .HasOne(com => com.Master)
                .WithMany(cli => cli.Comments)
                .HasForeignKey(com => com.MasterId);
        }
    }
}
