using Npgsql;

namespace BlazorApp1.Data
{
    // Создаёт схему и начальные данные (Database/*.sql), если в БД ещё нет таблиц.
    // Скрипты встроены в сборку, поэтому на сервере не нужны никакие файлы, кроме docker-compose.yml.
    public static class DatabaseInitializer
    {
        private const int MaxConnectAttempts = 30;
        private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(2);
        // Произвольный ключ, чтобы два экземпляра приложения не создавали схему одновременно
        private const long InitLockKey = 727_001;

        public static async Task InitializeAsync(DbConnectionFactory factory, ILogger logger)
        {
            await using var conn = await OpenWithRetryAsync(factory, logger);

            await ExecuteAsync(conn, $"SELECT pg_advisory_lock({InitLockKey})");
            try
            {
                await using (var check = new NpgsqlCommand("SELECT to_regclass('public.users') IS NOT NULL", conn))
                {
                    if ((bool)(await check.ExecuteScalarAsync())!)
                    {
                        logger.LogInformation("Database schema already exists");
                        return;
                    }
                }

                logger.LogInformation("Database is empty, creating schema and seed data");
                await using var tx = await conn.BeginTransactionAsync();
                foreach (var (name, sql) in LoadScripts())
                {
                    logger.LogInformation("Applying {Script}", name);
                    await ExecuteAsync(conn, sql);
                }
                await tx.CommitAsync();
                logger.LogInformation("Database initialized");
            }
            finally
            {
                await ExecuteAsync(conn, $"SELECT pg_advisory_unlock({InitLockKey})");
            }
        }

        private static async Task<NpgsqlConnection> OpenWithRetryAsync(DbConnectionFactory factory, ILogger logger)
        {
            for (int attempt = 1; ; attempt++)
            {
                var conn = factory.Create();
                try
                {
                    await conn.OpenAsync();
                    return conn;
                }
                catch (Exception ex) when (ex is NpgsqlException or System.Net.Sockets.SocketException && attempt < MaxConnectAttempts)
                {
                    await conn.DisposeAsync();
                    logger.LogWarning("Database not reachable (attempt {Attempt}/{Max}): {Message}", attempt, MaxConnectAttempts, ex.Message);
                    await Task.Delay(RetryDelay);
                }
            }
        }

        // Встроенные ресурсы Database/*.sql в порядке имён (01-, 02-, 03-...)
        private static IEnumerable<(string Name, string Sql)> LoadScripts()
        {
            var assembly = typeof(DatabaseInitializer).Assembly;
            foreach (var resource in assembly.GetManifestResourceNames()
                         .Where(n => n.Contains(".Database.") && n.EndsWith(".sql"))
                         .OrderBy(n => n, StringComparer.Ordinal))
            {
                using var stream = assembly.GetManifestResourceStream(resource)!;
                using var reader = new StreamReader(stream);
                yield return (resource, reader.ReadToEnd());
            }
        }

        private static async Task ExecuteAsync(NpgsqlConnection conn, string sql)
        {
            await using var cmd = new NpgsqlCommand(sql, conn);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
