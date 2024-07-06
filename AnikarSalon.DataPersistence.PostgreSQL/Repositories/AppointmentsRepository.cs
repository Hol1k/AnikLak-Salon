using AnikarSalon.Persistence.Postgres;

namespace AnikarSalon.DataPersistence.PostgreSQL.Repositories
{
    public class AppointmentsRepository
    {
        private readonly SalonDbContext _dbContext;

        public AppointmentsRepository(SalonDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
