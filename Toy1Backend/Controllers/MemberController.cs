using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toy1Backend.Auth;

namespace Toy1Backend.Controllers
{
    [ApiController]
    [Authorize(Policy = PolicyNames.MemberOrAdmin)]
    public class MemberController : ControllerBase
    {
        [HttpGet("membertest")]
        public async Task<IActionResult> AuthTest()
        {
            return Ok("OK Member or Admin!!!");
        }
    }
}
