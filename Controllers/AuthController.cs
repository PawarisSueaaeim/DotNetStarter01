using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication6.Data;
using WebApplication6.Models;
using WebApplication6.Models.Entities;

namespace WebApplication6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static Auth user = new Auth();

        [HttpPost]
        [Route("register")]
        public ActionResult<Auth> Register(AuthDto request)
        {
            var hashPassword = new PasswordHasher<Auth>().HashPassword(user, request.Password);
            user.Username = request.Username;
            user.PasswordHash = hashPassword;
            
            return Ok(user);
        }

        [HttpPost]
        [Route("login")]
        public ActionResult<string> Login(AuthDto request)
        {
            if (request.Username != user.Username)
            {
                return BadRequest("Username not found");
            }
            if (new PasswordHasher<Auth>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                return BadRequest("Password Invalid");
            }
            string token = "Success";

            return Ok(token);
        }

        private 
    }
}
