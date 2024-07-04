using AnikarSalon.Persistence.Postgres.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnikarSalon.Persistence.Postgres.Configurations
{
    public class ClientConfiguration : IEntityTypeConfiguration<ClientEntity>
    {
        public void Configure(EntityTypeBuilder<ClientEntity> builder)
        {
            builder.HasKey(cli => cli.Id);

            builder
                .HasMany(cli => cli.Appointments)
                .WithOne(app => app.Client)
                .HasForeignKey(app => app.ClientId);

            builder
                .HasMany(cli => cli.Comments)
                .WithOne(com => com.Author)
                .HasForeignKey(com => com.AuthorId);
        }
    }
}
