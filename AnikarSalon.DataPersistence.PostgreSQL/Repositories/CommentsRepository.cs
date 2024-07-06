using AnikarSalon.Persistence.Postgres;

namespace AnikarSalon.DataPersistence.PostgreSQL.Repositories
{
    public class CommentsRepository
    {
        private readonly SalonDbContext _dbContext;

        public CommentsRepository(SalonDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
