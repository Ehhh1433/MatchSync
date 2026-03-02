using Microsoft.AspNetCore.Mvc;
using MatchSync.Models;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly MatchSyncContext _context;

    public AuthController(MatchSyncContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(User newUser)
    {
        // 1. Basic Security: Check if phone already exists
        if (_context.Users.Any(u => u.PhoneNumber == newUser.PhoneNumber))
        {
            return BadRequest("This phone number is already registered.");
        }

        // 2. Add to Database
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        // 3. Simulated OTP Logic
        // In a real app, you'd call an SMS API here
        return Ok(new { message = "Registration successful! Please verify your OTP." });
    }
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // 1. Find the user by phone number or email
        var user = _context.Users.FirstOrDefault(u => u.PhoneNumber == request.Username || u.Email == request.Username);

        if (user == null || user.PasswordHash != request.Password)
        {
            return Unauthorized("Invalid phone number/email or password.");
        }

        // 2. FIXED: Check if the account is verified via OTP
        if (!user.IsVerified)
        {
            // Use StatusCode(403, ...) instead of Forbidden()
            return StatusCode(403, "Please verify your account via OTP before logging in.");
        }

        // 3. Return user info and RoleID to the app
        return Ok(new
        {
            Message = "Login successful!",
            UserID = user.UserID,
            FullName = user.FullName,
            Role = user.RoleID // 0 = Admin, 1 = Staff, 2 = Customer
        });
    }
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp(string phoneNumber, string otpCode)
    {
        var user = _context.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        // Simulated OTP Check (Hardcoded as '1234' for testing)
        if (otpCode == "1234")
        {
            user.IsVerified = true;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Account verified successfully! You can now login." });
        }

        return BadRequest("Invalid OTP code.");
    }
    [HttpPut("update-role/{targetUserId}")]
    public async Task<IActionResult> UpdateUserRole(int targetUserId, int newRoleId)
    {
        // Logic: Only an Admin (Role 0) should be able to call this!
        var user = await _context.Users.FindAsync(targetUserId);
        if (user == null) return NotFound();

        user.RoleID = newRoleId; // 0=Admin, 1=Staff, 2=Customer
        await _context.SaveChangesAsync();

        return Ok(new { message = "User role updated successfully." });
    }

    // FIXED: Added = string.Empty; to stop the CS8618 warnings
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}