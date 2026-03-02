namespace MatchSync.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty; // Used for OTP
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int RoleID { get; set; } = 2; // Default: 2 (Customer)
        public bool IsVerified { get; set; } = false; // Becomes true after OTP
    }
}