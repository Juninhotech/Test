using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Test.Repository;

namespace Test.Controllers
{
    [Route("api/jobs")]
    [ApiController]
    public class EmailController : ControllerBase
    {


        [HttpPost("enqueue")]
        public IActionResult EnqueueJob(string EmailTo, string name)
        {
            BackgroundJob.Enqueue<EmailService>(x => x.SendEmail(EmailTo, name));
            return Ok("Email job enqueued!");
        }

        [HttpPost("schedule")]
        public IActionResult ScheduleJob(string EmailTo, string name)
        {
            BackgroundJob.Schedule<EmailService>(x => x.SendEmail(EmailTo, name), TimeSpan.FromMinutes(1));
            return Ok("Email job scheduled!");
        }
    }
}
