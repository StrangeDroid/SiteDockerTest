﻿namespace BlazorApp1.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int Coins { get; set; }
        public List<Flower> Flowers { get; set; } = new(); // 🌸 вот это поле
    }
}