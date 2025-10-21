namespace ServiceManagementSystem.Infrastructure.Data
{
    public interface IDatabaseInitializer
    {
        Task InitializeAsync();
    }
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IDatabaseConnection _database;

        public DatabaseInitializer(IDatabaseConnection database)
        {
            _database = database;
        }

        public async Task InitializeAsync()
        {
            var createTablesSql = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ServiceProviders' AND xtype='U')
            CREATE TABLE ServiceProviders (
                Id INT PRIMARY KEY IDENTITY(1,1),
                Name NVARCHAR(100) NOT NULL,
                Email NVARCHAR(100),
                Phone NVARCHAR(20),
                Address NVARCHAR(200),
                CreatedDate DATETIME2 DEFAULT GETDATE()
            );

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Products' AND xtype='U')
            CREATE TABLE Products (
                Id INT PRIMARY KEY IDENTITY(1,1),
                ProductName NVARCHAR(100) NOT NULL,
                Price DECIMAL(18,2) NOT NULL,
                CreationDate DATETIME2  NOT NULL,
                ServiceProviderId INT NOT NULL,
                FOREIGN KEY (ServiceProviderId) REFERENCES ServiceProviders(Id)
            );";

            try
            {
                await _database.ExecuteNonQueryAsync(createTablesSql);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization failed: {ex.Message}");
            }
        }
    }
}
