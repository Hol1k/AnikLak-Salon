using AnikarSalon.Persistence.Postgres.Configurations;
using AnikarSalon.Persistence.Postgres.Models;
using Microsoft.EntityFrameworkCore;

namespace AnikarSalon.Persistence.Postgres
{
    public class SalonDbContext(DbContextOptions<SalonDbContext> options) : DbContext(options)
    {
        public DbSet<AppointmentEntity> Appointments { get; set; }
        public DbSet<ClientEntity> Clients { get; set; }
        public DbSet<CommentEntity> Comments { get; set; }
        public DbSet<MasterEntity> Masters { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
            modelBuilder.ApplyConfiguration(new ClientConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
            modelBuilder.ApplyConfiguration(new MasterConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
