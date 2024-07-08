using AnikarSalon.Persistence.Postgres;
using AnikarSalon.Persistence.Postgres.Models;
using Microsoft.EntityFrameworkCore;

namespace AnikarSalon.DataPersistence.PostgreSQL.Repositories
{
    public class ClientsRepository
    {
        private readonly SalonDbContext _dbContext;

        public ClientsRepository(SalonDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(string firstName, string password, string phoneNumber, string lastName = "")
        {
            ClientEntity newClient = new ClientEntity()
            {
                FirstName = firstName,
                LastName = lastName,
                Password = password,
                PhoneNumber = phoneNumber
            };

            await _dbContext.AddAsync(newClient);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Add(ClientEntity newClient)
        {
            await _dbContext.AddAsync(newClient);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ClientEntity?> Login(string phoneNumber, string password)
        {
            ClientEntity? client = await _dbContext.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);

            if (client == null || client.Password != password) return null;

            return client;
        }

        public async Task<bool> IsNumberOccupied(string phoneNumber)
        {
            ClientEntity? client = await _dbContext.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);

            if (client != null) return true;
            return false;
        }
    }
}
