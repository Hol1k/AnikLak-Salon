using AnikarSalon.Persistence.Postgres;
using AnikarSalon.Persistence.Postgres.Models;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AnikarSalon.DataPersistence.PostgreSQL.Repositories
{
    public class AppointmentsRepository
    {
        private readonly SalonDbContext _dbContext;

        public AppointmentsRepository(SalonDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> Registrate(string date, string time, string clientId, string masterId, string appointmentName)
        {
            string[] timeArr = time.Split(':');

            DateTime formatedDateTime = new(
                int.Parse(date.Remove(4)),
                int.Parse(date.Remove(0, 5).Remove(2)),
                int.Parse(date.Remove(0, 8)),
                int.Parse(timeArr[0]),
                int.Parse(timeArr[1]),
                0);

            return await Registrate(formatedDateTime, clientId, masterId, appointmentName);
        }

        public async Task<Guid> Registrate(DateTime dateTime, string clientId, string masterId, string appointmentName)
        {
            int appointmentDurationByHalfHours;
            decimal price;

            (appointmentDurationByHalfHours, price) = AppointmentData.Get(appointmentName);

            AppointmentEntity newAppointment = new AppointmentEntity()
            {
                DateTime = dateTime,
                AppointmentDurationByHalfHours = appointmentDurationByHalfHours,
                Status = "Обрабатывается",
                Price = price,
                AppointmentName = appointmentName,
                MasterId = Guid.Parse(masterId),
                ClientId = Guid.Parse(clientId)
            };

            await _dbContext.AddAsync(newAppointment);

            var master = await _dbContext.Masters
                .AsNoTracking()
                .Include(m => m.Appointments)
                .FirstOrDefaultAsync(m => m.Id.ToString() == masterId)
                ?? throw new Exception();

            master.Appointments.Add(newAppointment);

            var client = await _dbContext.Clients
                .AsNoTracking()
                .Include(c => c.Appointments)
                .FirstOrDefaultAsync(c => c.Id.ToString() == clientId)
                ?? throw new Exception();

            client.Appointments.Add(newAppointment);

            await _dbContext.SaveChangesAsync();

            return newAppointment.Id;
        }
    }
}
