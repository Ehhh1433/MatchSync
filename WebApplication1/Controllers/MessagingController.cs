using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchSync.Models;

namespace MatchSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagingController : ControllerBase
    {
        private readonly MatchSyncContext _context;
        public MessagingController(MatchSyncContext context) => _context = context;

        // Requirement: Admin can send messages/notifications to Staff
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] Message newMessage)
        {
            newMessage.SentAt = DateTime.Now;
            newMessage.IsRead = false;

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Message sent successfully!" });
        }

        // Requirement: Get conversation between two users (e.g., Admin and a specific Staff)
        [HttpGet("history/{userA}/{userB}")]
        public async Task<IActionResult> GetChatHistory(int userA, int userB)
        {
            var chat = await _context.Messages
                .Where(m => (m.SenderID == userA && m.ReceiverID == userB) ||
                            (m.SenderID == userB && m.ReceiverID == userA))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return Ok(chat);
        }

        // Requirement: Notification Function (Get unread count for Dashboard)
        [HttpGet("notifications/unread/{userId}")]
        public async Task<IActionResult> GetUnreadCount(int userId)
        {
            var count = await _context.Messages
                .CountAsync(m => m.ReceiverID == userId && !m.IsRead);
            return Ok(new { UnreadCount = count });
        }
    }
}