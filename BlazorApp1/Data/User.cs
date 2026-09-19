namespace BlazorApp1.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int Coins { get; set; }
        public bool IsNewUser { get; set; }
        public List<Flower> Flowers { get; set; } = new(); // 🌸 вот это поле
    }
    public interface ICurrentUserService
    {
        User? User { get; set; }
    }
        public class CurrentUserService : ICurrentUserService
        {
            public User? User { get; set; }
        }
}