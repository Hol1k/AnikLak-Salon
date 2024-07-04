using System.Reflection.Emit;

namespace AnikarSalon.Persistence.Postgres.Models
{
    public class CommentEntity
    {
        public Guid Id { get; set; }

        public Guid AuthorId { get; set; }

        public ClientEntity? Author { get; set; }

        public DateTime DateTime { get; set; } = DateTime.Now;

        public Guid MasterId { get; set; }

        public MasterEntity? Master { get; set; }
    }
}
