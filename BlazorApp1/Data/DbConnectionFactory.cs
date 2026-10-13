using Npgsql;

namespace BlazorApp1.Data
{
    public class DbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");
        }

        public NpgsqlConnection Create() => new NpgsqlConnection(_connectionString);
    }
}
