using Microsoft.Data.SqlClient;

namespace ServiceManagementSystem.Infrastructure.Data
{
    public interface IDatabaseInitializer
    {
        Task<bool> InitializeAsync();
        Task<bool> DatabaseExistsAsync();
    }

    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IDatabaseConnection _database;
        private readonly Func<string> _connectionStringProvider;

        public DatabaseInitializer(IDatabaseConnection database, Func<string> connectionStringProvider)
        {
            _database = database;
            _connectionStringProvider = connectionStringProvider;
        }

        public async Task<bool> DatabaseExistsAsync()
        {
            var databaseName = GetDatabaseNameFromConnectionString();
            var checkDbSql = $"SELECT database_id FROM sys.databases WHERE name = '{databaseName}'";

            try
            {
                var result = await _database.ExecuteScalarAsync(checkDbSql);
                return result != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> InitializeAsync()
        {
            // Create Database if not Exists
            if (!await DatabaseExistsAsync())
            {
                if (!await CreateDatabaseAsync())
                {
                    return false;
                }
            }

            // Create Tables if not Exists
            return await CreateTablesAsync();
        }

        private async Task<bool> CreateDatabaseAsync()
        {
            var databaseName = GetDatabaseNameFromConnectionString();
            var masterConnectionString = GetMasterConnectionString();

            var createDbSql = $@"
                IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{databaseName}')
                BEGIN
                    CREATE DATABASE [{databaseName}];
                END";

            try
            {
                using var masterConnection = new SqlConnection(masterConnectionString);
                await masterConnection.OpenAsync();
                using var command = new SqlCommand(createDbSql, masterConnection);
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create database: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> CreateTablesAsync()
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
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Table creation failed: {ex.Message}");
                return false;
            }
        }

        private string GetDatabaseNameFromConnectionString()
        {
            var builder = new SqlConnectionStringBuilder(_connectionStringProvider());
            return builder.InitialCatalog;
        }

        private string GetMasterConnectionString()
        {
            var builder = new SqlConnectionStringBuilder(_connectionStringProvider())
            {
                InitialCatalog = "master"
            };
            return builder.ToString();
        }
    }
}