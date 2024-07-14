using AnikarSalon.Persistence.Postgres;
using AnikarSalon.Persistence.Postgres.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace AnikarSalon.DataPersistence.PostgreSQL.Repositories
{
    public class MastersRepository
    {
        private readonly SalonDbContext _dbContext;

        public MastersRepository(SalonDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<MasterEntity>> GetAll()
        {
            return await _dbContext.Masters
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<string>> GetFreeRegistrationTimes(DateOnly date, string masterId)
        {
            var master = await _dbContext.Masters
                .AsNoTracking()
                .Include(m => m.Appointments)
                .FirstOrDefaultAsync(m => m.Id.ToString() == masterId);
            if (master == null) return [];

            List<string> occupiedTimes = [];
            foreach (var appointment in master.Appointments)
            {
                if (appointment.Status == "Отменен" || appointment.Status == "Выполнен") continue;

                DateOnly appointmentDate = new(appointment.DateTime.Year, appointment.DateTime.Month, appointment.DateTime.Day);

                if (appointmentDate != date) continue;

                TimeOnly appointmentTime = new(appointment.DateTime.Hour, appointment.DateTime.Minute);
                for (int i = 0; i < appointment.AppointmentDurationByHalfHours; i++)
                {
                    string regMinute = appointmentTime.Minute == 0 ? "00" : appointmentTime.Minute.ToString();
                    occupiedTimes.Add($"{appointmentTime.Hour}:{regMinute}");
                    appointmentTime = appointmentTime.AddMinutes(30);
                }
            }

            return occupiedTimes;
        }

        public async Task<List<string>> GetFreeRegistrationTimes(string date, string masterId)
        {
            DateOnly formatedDate = new(
                int.Parse(date.Remove(4)),
                int.Parse(date.Remove(0, 5).Remove(2)),
                int.Parse(date.Remove(0, 8)));

            return await GetFreeRegistrationTimes(formatedDate, masterId);
        }

        public async Task<MasterEntity?> Login (string phoneNumber, string password)
        {
            MasterEntity? master = await _dbContext.Masters
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);

            if (master == null || master.Password != password) return null;

            return master;
        }

        public async Task<List<AppointmentEntity>> GetAllAppointments(string masterId)
        {
            return await GetAllAppointments(Guid.Parse(masterId));
        }

        public async Task<List<AppointmentEntity>> GetAllAppointments(Guid masterId)
        {
            MasterEntity? client = await _dbContext.Masters
               .AsNoTracking()
               .Include(c => c.Appointments)
               .ThenInclude(a => a.Client)
               .FirstOrDefaultAsync(c => c.Id == masterId);

            if (client == null) return [];

            return client.Appointments.OrderBy(c => c.DateTime).ToList();
        }
    }
}
