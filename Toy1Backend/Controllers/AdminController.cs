using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toy1Backend.Auth;

namespace Toy1Backend.Controllers
{
    [ApiController]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    public class AdminController : ControllerBase
    {
        [HttpGet("admintest")]
        public async Task<IActionResult> AuthTest()
        {
            return Ok("OK Admin!!!");
        }
    }
}
