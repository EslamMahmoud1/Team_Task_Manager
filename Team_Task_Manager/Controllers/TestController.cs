using Microsoft.AspNetCore.Mvc;
using Team_Task_Manager.Services.Interfaces;
using Team_Task_Manager.Shared;

namespace Team_Task_Manager.Controllers
{
    public class TestController : Controller
    {
        private readonly IEmailService _emailService;
        public TestController(IEmailService emailService)
        {
            _emailService = emailService
                ?? throw new ArgumentNullException(nameof(emailService));
        }
        [HttpGet("singleemail")]
        public async Task<IActionResult> SendSingleEmail()
        {
            EmailMetadata emailMetadata = new("kokoeslam62@gmail.com",
                "FluentEmail Test email",
                "This is a test email from FluentEmail.");
            await _emailService.Send(emailMetadata);
            return Ok();
        }
    }
}
