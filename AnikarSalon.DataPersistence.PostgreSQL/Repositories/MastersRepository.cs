using AnikarSalon.Persistence.Postgres;

namespace AnikarSalon.DataPersistence.PostgreSQL.Repositories
{
    public class MastersRepository
    {
        private readonly SalonDbContext _dbContext;

        public MastersRepository(SalonDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
