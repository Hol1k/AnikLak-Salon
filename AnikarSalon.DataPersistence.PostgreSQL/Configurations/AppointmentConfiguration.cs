using AnikarSalon.Persistence.Postgres.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnikarSalon.Persistence.Postgres.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<AppointmentEntity>
    {
        public void Configure(EntityTypeBuilder<AppointmentEntity> builder)
        {
            builder.HasKey(app => app.Id);

            builder
                .HasOne(app => app.Master)
                .WithMany(mas => mas.Appointments)
                .HasForeignKey(app => app.MasterId);

            builder
                .HasOne(app => app.Client)
                .WithMany(cli => cli.Appointments)
                .HasForeignKey(app => app.ClientId);
        }
    }
}
