using AnikarSalon.Persistence.Postgres.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnikarSalon.Persistence.Postgres.Configurations
{
    public class MasterConfiguration : IEntityTypeConfiguration<MasterEntity>
    {
        public void Configure(EntityTypeBuilder<MasterEntity> builder)
        {
            builder.HasKey(mas => mas.Id);

            builder
                .HasMany(mas => mas.Appointments)
                .WithOne(app => app.Master)
                .HasForeignKey(app => app.MasterId);

            builder
                .HasMany(mas => mas.Orders)
                .WithOne(ord => ord.Customer)
                .HasForeignKey(app => app.CustomerId);

            builder
                .HasMany(mas => mas.Comments)
                .WithOne(com => com.Master)
                .HasForeignKey(com => com.MasterId);
        }
    }
}
