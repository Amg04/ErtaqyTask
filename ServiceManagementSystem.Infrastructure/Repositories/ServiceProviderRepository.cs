using Microsoft.Data.SqlClient;
using ServiceManagementSystem.Core.Entities;
using ServiceManagementSystem.Core.Interfaces;
using ServiceManagementSystem.Infrastructure.Data;
using System.Data;

namespace ServiceManagementSystem.Infrastructure.Repositories
{
    public class ServiceProviderRepository : IServiceProviderRepository
    {
        private readonly IDatabaseConnection _database;

        public ServiceProviderRepository(IDatabaseConnection database)
        {
            _database = database;
        }

        public async Task<IEnumerable<ServiceProvider>> GetAllAsync()
        {
            var serviceProviders = new List<ServiceProvider>();
            var sql = @"SELECT Id, Name, Email, Phone, Address, CreatedDate 
                       FROM ServiceProviders 
                       ORDER BY CreatedDate DESC";

            using var reader = await _database.ExecuteReaderAsync(sql);

            while (await reader.ReadAsync())
            {
                serviceProviders.Add(MapToServiceProvider(reader));
            }

            return serviceProviders;
        }

        public async Task AddAsync(ServiceProvider serviceProvider)
        {
            var sql = @"INSERT INTO ServiceProviders (Name, Email, Phone, Address, CreatedDate) 
                       VALUES (@Name, @Email, @Phone, @Address, @CreatedDate)";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Name", serviceProvider.Name),
                new SqlParameter("@Email", serviceProvider.Email ?? (object)DBNull.Value),
                new SqlParameter("@Phone", serviceProvider.Phone ?? (object)DBNull.Value),
                new SqlParameter("@Address", serviceProvider.Address ?? (object)DBNull.Value),
                new SqlParameter("@CreatedDate", serviceProvider.CreatedDate)
            };

            await _database.ExecuteNonQueryAsync(sql, parameters);
        }

        private ServiceProvider MapToServiceProvider(SqlDataReader reader)
        {
            return new ServiceProvider
            {
                Id = reader.GetInt32("Id"),
                Name = reader.GetString("Name"),
                Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                Phone = reader.IsDBNull("Phone") ? null : reader.GetString("Phone"),
                Address = reader.IsDBNull("Address") ? null : reader.GetString("Address"),
                CreatedDate = reader.GetDateTime("CreatedDate")
            };
        }
    }
}