using AnikarSalon.Persistence.Postgres;

namespace AnikarSalon.DataPersistence.PostgreSQL.Repositories
{
    public class ClientsRepository
    {
        private readonly SalonDbContext _dbContext;

        public ClientsRepository(SalonDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
