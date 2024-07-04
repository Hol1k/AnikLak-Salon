namespace AnikarSalon.Persistence.Postgres.Models
{
    public class OrderEntity
    {
        public Guid Id { get; set; }

        public List<string> Items { get; set; } = [];

        public List<int> ItemsCount { get; set; } = [];

        public DateTime OrderDate { get; set; }

        public DateTime ArrivalDate { get; set; }

        public Guid CustomerId { get; set; }

        public MasterEntity? Customer { get; set; }
    }
}
