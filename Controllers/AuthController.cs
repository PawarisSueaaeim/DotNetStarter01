using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApplication6.Data;
using WebApplication6.Models;
using WebApplication6.Models.Entities;

namespace WebApplication6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IConfiguration configuretion, ApplicationDbContext dbContext) : ControllerBase
    {
        

        [HttpPost]
        [Route("register")]
        public ActionResult<Auth> Register(AuthDto request)
        {

            var hasUser = dbContext.Auths.FirstOrDefault(u => u.Username == request.Username);
            if (hasUser != null)
            {
                return BadRequest("User is already!!");
            }
            var user = new Auth();
            user.Username = request.Username;
            user.PasswordHash = new PasswordHasher<Auth>().HashPassword(user, request.Password);
           
            dbContext.Auths.Add(user);
            dbContext.SaveChanges();
            
            return Ok(user);
        }

        [HttpPost]
        [Route("login")]
        public ActionResult<Auth> Login(AuthDto request)
        {
            var hasUser = dbContext.Auths.FirstOrDefault(u => u.Username == request.Username);
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
                refresh_token = GenerateAndSaveRefreshToken(hasUser),
                access_token = CreateToken(hasUser)
            };
         
            return Ok(response);
        }

        //[HttpPost]
        //[Route("refresh-token")]
        //public async Task<ActionResult<Auth>> RefreshToken(RefreshTokenDto request)
        //{
        //    var hasUser = dbContext.Auths.Find(request.UserId);
        //    if (hasUser is null)
        //    {
        //        return BadRequest("User NotFound");
        //    }
        //    if (request.RefreshToken == hasUser.RefreshToken)
            
        //    return Ok();
        //}


        [HttpPost]
        [Route("refresh-token")]
        private async Task<string> GenerateAndSaveRefreshToken(Auth user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshExpriryToken = DateTime.UtcNow.AddDays(7);
            await dbContext.SaveChangesAsync();

            return refreshToken;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private string CreateToken(Auth user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuretion.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var toKenDescriptor = new JwtSecurityToken(
                    issuer: configuretion.GetValue<string>("AppSettings:Issuer"),
                    audience: configuretion.GetValue<string>("AppSettings:Audience"),
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(toKenDescriptor);
        }
    }
}
