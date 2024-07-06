using AnikarSalon.Persistence.Postgres;

namespace AnikarSalon.DataPersistence.PostgreSQL.Repositories
{
    public class OrdersRepository
    {
        private readonly SalonDbContext _dbContext;

        public OrdersRepository(SalonDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
