namespace AnikarSalon.Persistence.Postgres.Models
{
    public class AppointmentEntity
    {
        public Guid Id { get; set; }

        public DateTime DateTime { get; set; } = DateTime.Now;

        public string Status { get; set; } = string.Empty;

        public decimal Price { get; set; } = 0;

        public string AppointmentName { get; set; } = string.Empty;

        public Guid MasterId { get; set; }

        public MasterEntity? Master { get; set; }

        public Guid ClientId { get; set; }

        public ClientEntity? Client { get; set; }
    }
}
