using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace MatchSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimulationController : ControllerBase
    {
        // Requirement: Simulate Receipt as note.txt with QR Code
        [HttpPost("generate-receipt")]
        public IActionResult GenerateReceipt([FromBody] ReceiptRequest req)
        {
            string fileName = "receipt_note.txt";
            string content = $@"
==========================================
        MATCHSYNC SPORTS COMPLEX
==========================================
Ref Code: MS-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}
Date: {req.Date}
Time Slot: {req.Slot}:00
Court: {req.CourtName}
Total Paid: {req.Amount} PHP
------------------------------------------
QR CODE DATA: {req.QRCodeToken}
------------------------------------------
Thank you for booking with MatchSync!
==========================================";

            System.IO.File.WriteAllText(fileName, content);
            return Ok(new { Message = "Receipt simulated in receipt_note.txt", Path = Path.GetFullPath(fileName) });
        }

        // Requirement: Simulate OTP as note.txt
        [HttpPost("generate-otp")]
        public IActionResult GenerateOtp(string phoneNumber)
        {
            string otp = "1234"; // Simulated static OTP
            string content = $"MatchSync Verification Code for {phoneNumber}: {otp}";

            System.IO.File.WriteAllText("otp_note.txt", content);
            return Ok(new { Message = "OTP simulated in otp_note.txt" });
        }
    }

    public class ReceiptRequest
    {
        public string Date { get; set; } = "";
        public int Slot { get; set; }
        public string CourtName { get; set; } = "";
        public decimal Amount { get; set; }
        public string QRCodeToken { get; set; } = "";
    }
}