using MySql.Data.MySqlClient;

namespace BlazorApp1.Data
{
    public static class UserRepository
    {
        public static async Task<User> LoadUserById(int userId, MySqlConnection conn)
        {
            var user = new User();

            // 1. Получаем пользователя
            var userCmd = new MySqlCommand("SELECT id, username, email, password_hash, coins FROM users WHERE id = @uid", conn);
            userCmd.Parameters.AddWithValue("@uid", userId);
            using var reader = await userCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                user.Id = reader.GetInt32(0);
                user.Username = reader.GetString(1);
                user.Email = reader.GetString(2);
                user.PasswordHash = reader.GetString(3);
                user.Coins = reader.GetInt32(4);
            }
            await reader.CloseAsync();

            // 2. Загружаем цветы + данные типов
            var flowers = new List<Flower>();
            var flowerCmd = new MySqlCommand(@"
        SELECT f.id, f.hp, f.pot_slot, f.type_id, t.name, t.code, t.price_buy, t.price_sell
        FROM flowers f
        JOIN flowertypes t ON f.type_id = t.id
        WHERE f.user_id = @uid", conn);
            flowerCmd.Parameters.AddWithValue("@uid", userId);
            using var fReader = await flowerCmd.ExecuteReaderAsync();
            while (await fReader.ReadAsync())
            {
                flowers.Add(new Flower
                {
                    Id = fReader.GetInt32(0),              // ← теперь тоже читаем ID цветка
                    Hp = fReader.GetInt32(1),
                    MaxHp = 15,
                    Slot = fReader.GetInt32(2),
                    TypeId = fReader.GetInt32(3),
                    DisplayName = fReader.GetString(4),
                    CodeName = fReader.GetString(5),
                    PriceBuy = fReader.GetInt32(6),        // ← добавлено
                    PriceSell = fReader.GetInt32(7)        // ← добавлено
                });
            }

            user.Flowers = flowers;
            return user;
        }

    }

}
