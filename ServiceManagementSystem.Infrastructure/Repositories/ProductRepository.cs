using Microsoft.Data.SqlClient;
using ServiceManagementSystem.Core.Entities;
using ServiceManagementSystem.Core.Interfaces;
using ServiceManagementSystem.Infrastructure.Data;
using System.Data;

namespace ServiceManagementSystem.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDatabaseConnection _database;

        public ProductRepository(IDatabaseConnection database)
        {
            _database = database;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var products = new List<Product>();
            var sql = @"SELECT p.Id, p.ProductName, p.Price, p.ServiceProviderId, p.CreationDate,
                               sp.Id as ProviderId, sp.Name as ProviderName, sp.Email, sp.Phone, sp.Address
                        FROM Products p
                        INNER JOIN ServiceProviders sp ON p.ServiceProviderId = sp.Id
                        ORDER BY p.CreationDate DESC";

            using var reader = await _database.ExecuteReaderAsync(sql);

            while (await reader.ReadAsync())
            {
                products.Add(MapToProduct(reader));
            }

            return products;
        }

        public async Task AddAsync(Product product)
        {
            var sql = @"INSERT INTO Products (ProductName, Price, ServiceProviderId, CreationDate) 
                       VALUES (@ProductName, @Price, @ServiceProviderId, @CreationDate)";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ProductName", product.ProductName),
                new SqlParameter("@Price", product.Price),
                new SqlParameter("@ServiceProviderId", product.ServiceProviderId),
                new SqlParameter("@CreationDate", product.CreationDate)
            };

            await _database.ExecuteNonQueryAsync(sql, parameters);
        }

     
        public async Task<IEnumerable<Product>> GetFilteredProductsAsync(decimal? minPrice, decimal? maxPrice,
            DateTime? fromDate, DateTime? toDate, int? serviceProviderId)
        {
            var products = new List<Product>();
            var sql = @"SELECT p.Id, p.ProductName, p.Price, p.ServiceProviderId, p.CreationDate,
                               sp.Id as ProviderId, sp.Name as ProviderName, sp.Email, sp.Phone, sp.Address
                        FROM Products p
                        INNER JOIN ServiceProviders sp ON p.ServiceProviderId = sp.Id
                        WHERE 1=1";

            var parameters = new List<SqlParameter>();

            if (minPrice.HasValue)
            {
                sql += " AND p.Price >= @MinPrice";
                parameters.Add(new SqlParameter("@MinPrice", minPrice.Value));
            }

            if (maxPrice.HasValue)
            {
                sql += " AND p.Price <= @MaxPrice";
                parameters.Add(new SqlParameter("@MaxPrice", maxPrice.Value));
            }

            if (fromDate.HasValue)
            {
                sql += " AND p.CreationDate >= @FromDate";
                parameters.Add(new SqlParameter("@FromDate", fromDate.Value));
            }

            if (toDate.HasValue)
            {
                sql += " AND p.CreationDate <= @ToDate";
                parameters.Add(new SqlParameter("@ToDate", toDate.Value));
            }

            if (serviceProviderId.HasValue && serviceProviderId > 0)
            {
                sql += " AND p.ServiceProviderId = @ServiceProviderId";
                parameters.Add(new SqlParameter("@ServiceProviderId", serviceProviderId.Value));
            }

            sql += " ORDER BY p.CreationDate DESC";

            using var reader = await _database.ExecuteReaderAsync(sql, parameters.ToArray());

            while (await reader.ReadAsync())
            {
                products.Add(MapToProduct(reader));
            }

            return products;
        }

        private Product MapToProduct(SqlDataReader reader)
        {
            var serviceProvider = new ServiceProvider
            {
                Id = reader.GetInt32("ProviderId"),
                Name = reader.GetString("ProviderName"),
                Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                Phone = reader.IsDBNull("Phone") ? null : reader.GetString("Phone"),
                Address = reader.IsDBNull("Address") ? null : reader.GetString("Address")
            };

            return new Product
            {
                Id = reader.GetInt32("Id"),
                ProductName = reader.GetString("ProductName"),
                Price = reader.GetDecimal("Price"),
                ServiceProviderId = reader.GetInt32("ServiceProviderId"),
                CreationDate = reader.GetDateTime("CreationDate"),
                ServiceProvider = serviceProvider
            };
        }
    }
}