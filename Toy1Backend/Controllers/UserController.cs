using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;
using System.Text;
using Toy1Backend.Auth;
using Toy1Backend.Db;
using Toy1Backend.Models;

namespace Toy1Backend.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        IMongoCollection<User> _users;
        string _cookieName;

        public UserController(DbCollection dbCollection, IConfiguration configuration)
        {
            _users = dbCollection.users;
            _cookieName = configuration.GetValue<string>("CookieName");
        }

        [HttpPost("login/{username}/{password}")]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = _users.Find(x => x.UserName == username).FirstOrDefault();
            if (user is null) return Unauthorized();

            var hasher = new PasswordHasher<string>();
            var verifyResult = hasher.VerifyHashedPassword(user.UserName, user.Password, password);
            if (verifyResult.Equals(PasswordVerificationResult.Failed)) return Unauthorized();

            var claims = new List<Claim>();
            if (user.UserClaim == "Admin")
            {
                claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimNames.Member, "true"),
                    new Claim(ClaimNames.Admin, "true")
                };
            }
            if (user.UserClaim == "Member")
            {
                claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimNames.Member, "true")
                };
            }

            var identity = new ClaimsIdentity(claims, _cookieName);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };
            await HttpContext.SignInAsync(_cookieName, claimsPrincipal, authProperties);
            return Ok(user);
        }

        [HttpPost("users")]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            if (user.UserClaim != ClaimNames.Member && user.UserClaim != ClaimNames.Admin)
                return BadRequest();

            var hasher = new PasswordHasher<string>();
            var hashedStr = hasher.HashPassword(user.UserName, user.Password);
            user.Password = hashedStr;

            _users.InsertOne(user);
            return Ok(user);
        }

        [HttpGet("users")]
        public async Task<IActionResult> Get()
        {
            var res = _users.Find(x => true).ToList();
            return Ok(res);
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var res = _users.Find(x => x.Id == id).FirstOrDefault();
            return Ok(res);
        }

        [HttpDelete("users")]
        public async Task<IActionResult> Delete()
        {
            var res = _users.DeleteMany(x => true);
            return Ok(res);
        }
    }
}
