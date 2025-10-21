using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace ServiceManagementSystem.Infrastructure.Data
{
    public interface IDatabaseConnection
    {
        Task<SqlConnection> CreateConnectionAsync();
        Task ExecuteNonQueryAsync(string sql, SqlParameter[] parameters = null);
        Task<object> ExecuteScalarAsync(string sql, SqlParameter[] parameters = null);
        Task<SqlDataReader> ExecuteReaderAsync(string sql, SqlParameter[] parameters = null);
    }

    public class DatabaseConnection : IDatabaseConnection
    {
        private readonly string _connectionString;

        public DatabaseConnection(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<SqlConnection> CreateConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        public async Task ExecuteNonQueryAsync(string sql, SqlParameter[] parameters = null)
        {
            try
            {
                await using var connection = await CreateConnectionAsync();
                await using var command = new SqlCommand(sql, connection);

                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new Exception($"Database error: {ex.Message}", ex);
            }
        }

        public async Task<object> ExecuteScalarAsync(string sql, SqlParameter[] parameters = null)
        {
            await using var connection = await CreateConnectionAsync();
            await using var command = new SqlCommand(sql, connection);

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            return await command.ExecuteScalarAsync();
        }

       

        public async Task<SqlDataReader> ExecuteReaderAsync(string sql, SqlParameter[] parameters = null)
        {
            var connection = await CreateConnectionAsync();
            var command = new SqlCommand(sql, connection);

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        }
    }
}
