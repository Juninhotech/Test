using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Test.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {

        [HttpGet("dashboard")]
        public IActionResult AdminPortal()
        {
            var userName = User.Identity?.Name; 

            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized(new { message = "User not logged in" });
            }

            return Ok(new { message = $"Welcome to the Admin portal, {userName}" });
        }
    }
}
