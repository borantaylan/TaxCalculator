using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TaxCalculator.API.Controllers
{
    //TEST CODE
    [ApiController]
    [Route("[controller]")]
    public class AuthorizationController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public AuthorizationController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet("token")]
        public IActionResult Get(string role = "ApiUser")
        {
            List<Claim> claims = new List<Claim>() {
                new Claim("sub","Bob"),
                new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", role),
            };

            var claimsIdentity = new ClaimsIdentity(claims);
            var tokenHandler = new JwtSecurityTokenHandler() { };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = configuration["AuthorizationSettings:Audience"],
                Issuer = "Tax Consultant XYZ Limited",
                Subject = claimsIdentity,
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(365),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AuthorizationSettings:SecretSymmetricKey"])), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(tokenHandler.WriteToken(token));
        }
    }
}
