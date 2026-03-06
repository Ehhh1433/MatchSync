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
            if (await _context.Users.AnyAsync(u => u.PhoneNumber == newUser.PhoneNumber))
                return BadRequest("Phone number already registered.");

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Registration successful! Use OTP 1234 to verify." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.Username || u.Email == request.Username);
            if (user == null || user.PasswordHash != request.Password) return Unauthorized("Invalid credentials.");
            if (!user.IsVerified) return StatusCode(403, "Please verify your account via OTP first.");

            return Ok(new { UserID = user.UserID, FullName = user.FullName, Role = user.RoleID });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(string phoneNumber, string otpCode)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            if (user == null) return NotFound("User not found.");
            if (otpCode == "1234")
            {
                user.IsVerified = true;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Account verified!" });
            }
            return BadRequest("Invalid OTP code.");
        }

        public class LoginRequest { public string Username { get; set; } = ""; public string Password { get; set; } = ""; }
    }
}