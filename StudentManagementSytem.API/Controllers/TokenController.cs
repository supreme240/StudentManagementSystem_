using ApplicationStudentManagement.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StudentManagementSystem.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Student_management_system.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public TokenController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("generate")]
        public IActionResult GenerateToken([FromBody] LogInAPIViewModel loginDto)
        {
            if (loginDto == null || string.IsNullOrWhiteSpace(loginDto.UserNameOrEmail)
                                 || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var user = _context.Registrations
                .FirstOrDefault(r => r.UserName == loginDto.UserNameOrEmail
                                  && r.Password == loginDto.Password);

            if (user == null)
                return Unauthorized(new { error = "Invalid authentication parameters provided." });

            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(
                _config["JwtSettings:Key"]!);
            //byte[] jwtSecretKey = RandomNumberGenerator.GetBytes(256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaims),
                Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_config["JwtSettings:ExpireDays"])),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var createdToken = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new
            {
                token = tokenHandler.WriteToken(createdToken),
                expiration = tokenDescriptor.Expires,
                role = user.Role
            });
        }
    }
}