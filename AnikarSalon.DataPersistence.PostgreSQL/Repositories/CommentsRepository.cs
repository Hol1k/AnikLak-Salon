using AnikarSalon.Persistence.Postgres;
using AnikarSalon.Persistence.Postgres.Models;
using Microsoft.EntityFrameworkCore;

namespace AnikarSalon.DataPersistence.PostgreSQL.Repositories
{
    public class CommentsRepository
    {
        private readonly SalonDbContext _dbContext;

        public CommentsRepository(SalonDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CommentEntity> WriteComment(Guid masterId, Guid clientId, string commentText)
        {
            CommentEntity newComment = new CommentEntity()
            {
                MasterId = masterId,
                AuthorId = clientId,
                Text = commentText
            };

            await _dbContext.AddAsync(newComment);

            var master = await _dbContext.Masters
                .AsNoTracking()
                .Include(m => m.Comments)
                .FirstOrDefaultAsync(m => m.Id == masterId)
                ?? throw new Exception();

            master.Comments.Add(newComment);

            var client = await _dbContext.Clients
                .AsNoTracking()
                .Include(c => c.Comments)
                .FirstOrDefaultAsync(c => c.Id == clientId)
                ?? throw new Exception();

            client.Comments.Add(newComment);

            await _dbContext.SaveChangesAsync();

            return newComment;
        }

        public async Task<List<CommentEntity>> GetComments(Guid masterId)
        {
            MasterEntity master = await _dbContext.Masters
                .AsNoTracking()
                .Include(m => m.Comments)
                .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(m => m.Id == masterId) ?? throw new Exception();

            return master.Comments;
        }
    }
}
