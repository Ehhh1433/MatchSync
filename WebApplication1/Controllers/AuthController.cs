using Microsoft.AspNetCore.Mvc;
using MatchSync.Models;
using Microsoft.EntityFrameworkCore;

namespace MatchSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MatchSyncContext _context;
        public AuthController(MatchSyncContext context) => _context = context;

        [HttpPost("register")]
        public async Task<IActionResult> Register(User newUser)
        {
            // Check by new Username field or Phone
            if (await _context.Users.AnyAsync(u => u.Username == newUser.Username || u.PhoneNumber == newUser.PhoneNumber))
                return BadRequest("Username or Phone number already exists.");

            newUser.LoyaltyPoints = 0;
            newUser.AccountStatus = "Active";

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Registration successful! Use OTP 1234 to verify." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || user.PasswordHash != request.Password)
                return Unauthorized("Invalid credentials.");

            if (user.AccountStatus == "Deleted")
                return Unauthorized("Account no longer exists.");

            return Ok(new { UserID = user.UserID, FullName = user.FullName, Role = user.RoleID, Points = user.LoyaltyPoints });
        }
        [HttpPost("create-staff")]
        public async Task<IActionResult> CreateStaff([FromBody] User staffUser)
        {
            // Force role to Staff
            staffUser.RoleID = 1;
            staffUser.AccountStatus = "Active";

            if (await _context.Users.AnyAsync(u => u.Username == staffUser.Username))
                return BadRequest("Username already taken.");

            _context.Users.Add(staffUser);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Staff account created successfully!" });
        }

        public class LoginRequest { public string Username { get; set; } = ""; public string Password { get; set; } = ""; }
    }
}