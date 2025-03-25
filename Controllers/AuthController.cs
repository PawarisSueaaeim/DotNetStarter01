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
        private readonly AuthService _authService;
        private readonly ApplicationDbContext _dbContext;
        public AuthController(ApplicationDbContext dbContext, AuthService authService)
        {   
            _dbContext = dbContext;
            _authService = authService;
        }

        [HttpPost]
        [Route("register")]
        public ActionResult<Auth> Register(AuthDto request)
        {
            var hasUser = _dbContext.Auths.FirstOrDefault(u => u.Username == request.Username);
            if (hasUser != null)
            {
                return BadRequest("User already exists!!!");
            }
            var user = new Auth();
            user.Username = request.Username;
            user.PasswordHash = new PasswordHasher<Auth>().HashPassword(user, request.Password);
           
            _dbContext.Auths.Add(user);
            _dbContext.SaveChanges();
            
            return Ok(user);
        }

        [HttpPost]
        [Route("login")]
        public ActionResult<Auth> Login(AuthDto request)
        {
            var hasUser = _dbContext.Auths.FirstOrDefault(u => u.Username == request.Username);
            if (hasUser == null)
            {
                return BadRequest("Username not found");
            }
            if (new PasswordHasher<Auth>().VerifyHashedPassword(hasUser, hasUser.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                return BadRequest("Password Invalid");
            }

            var response = new
            {
                user = new
                {
                    username = hasUser.Username,
                    role = hasUser.Role,
                },
                refresh_token = _authService.GenerateAndSaveRefreshToken(hasUser),
                access_token = _authService.CreateToken(hasUser),
            };
         
            return Ok(response);
        }

        [HttpPost]
        [Route("refresh-token")]
        public ActionResult<object> RefreshToken(RefreshTokenDto request)
        {
            var hasUser = _dbContext.Auths.Find(request.UserId);
            if (hasUser is null)
            {
                return BadRequest("User NotFound");
            }
            else if (request.RefreshToken != hasUser.RefreshToken || hasUser.RefreshExpriryToken <= DateTime.UtcNow)
            {
                return BadRequest("Refresh token invalid, Please login");
            }
            var access_token = _authService.CreateToken(hasUser);

            return Ok(access_token);
        }
    }
}
