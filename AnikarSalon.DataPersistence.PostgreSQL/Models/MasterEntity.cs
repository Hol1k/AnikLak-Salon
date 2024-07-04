namespace AnikarSalon.Persistence.Postgres.Models
{
    public class MasterEntity
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName {  get; set; } = string.Empty;

        public string Surname {  get; set; } = string.Empty;

        public string AvatarUrl {  get; set; } = string.Empty;

        public string PhoneNumber {  get; set; } = string.Empty;

        public string Password {  get; set; } = string.Empty;

        public string About {  get; set; } = string.Empty;

        public int YearsExperience { get; set; } = 0;

        public string[] Services {  get; set; } = [];

        public List<AppointmentEntity> Appointments { get; set; } = [];

        public List<OrderEntity> Orders { get; set; } = [];

        public List<CommentEntity> Comments { get; set; } = [];
    }
}
