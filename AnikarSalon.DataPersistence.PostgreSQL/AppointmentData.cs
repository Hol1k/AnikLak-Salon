namespace AnikarSalon.DataPersistence.PostgreSQL
{
    public static class AppointmentData
    {
        public static (int duration, decimal price) Get(string AppointmentName)
        {
            if (AppointmentName == "Маникюр с дизайном") return (6, 1500);
            if (AppointmentName == "Уходовый") return (1, 600);
            return (0, 0);
        }
    }
}
