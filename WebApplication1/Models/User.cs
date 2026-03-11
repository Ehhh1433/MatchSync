namespace MatchSync.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty; // New unique field
        public string PasswordHash { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public int RoleID { get; set; } = 2; // 0:Admin, 1:Staff, 2:User
        public int LoyaltyPoints { get; set; } = 0; // New field
        public string AccountStatus { get; set; } = "Active"; // New field
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}