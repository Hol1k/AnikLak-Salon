using AnikarSalon.Persistence.Postgres.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnikarSalon.Persistence.Postgres.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<OrderEntity>
    {
        public void Configure(EntityTypeBuilder<OrderEntity> builder)
        {
            builder.HasKey(ord => ord.Id);

            builder
                .HasOne(ord => ord.Customer)
                .WithMany(mas => mas.Orders)
                .HasForeignKey(ord => ord.CustomerId);
        }
    }
}
