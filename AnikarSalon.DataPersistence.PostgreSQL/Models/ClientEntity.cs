namespace AnikarSalon.Persistence.Postgres.Models
{
    public class ClientEntity
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Password {  get; set; } = string.Empty;

        public string PhoneNumber {  get; set; } = string.Empty;

        public List<AppointmentEntity> Appointments { get; set; } = [];

        public List<CommentEntity> Comments { get; set; } = [];
    }
}
