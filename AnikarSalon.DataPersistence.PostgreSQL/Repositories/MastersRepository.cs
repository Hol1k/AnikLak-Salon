using AnikarSalon.Persistence.Postgres;
using AnikarSalon.Persistence.Postgres.Models;
using Microsoft.EntityFrameworkCore;

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
                DateOnly appointmentDate = new(appointment.DateTime.Year, appointment.DateTime.Month, appointment.DateTime.Day);

                if (appointmentDate != date) continue;

                TimeOnly appointmentTime = new(appointment.DateTime.Hour, appointment.DateTime.Minute);
                for (int i = 0; i < appointment.AppointmentDurationByHalfHours; i++)
                {
                    occupiedTimes.Append($"{appointmentTime.Hour}:{appointmentTime.Minute}");
                    appointmentTime.AddMinutes(30);
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
    }
}
