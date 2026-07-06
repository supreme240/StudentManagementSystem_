using ApplicationStudentManagement.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StudentManagementSystem.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Student_management_system.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public TokenController(ApplicationDbContext context, IConfiguration config, IWebHostEnvironment env)
        {
            _context = context;
            _config = config;
            _env = env;
        }

        [HttpPost("generate")]
        public IActionResult GenerateToken([FromBody] LogInAPIViewModel loginDto)
        {
            if (loginDto == null || string.IsNullOrWhiteSpace(loginDto.UserNameOrEmail)
                                 || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest("Username and password are required.");
            }

            // validate credentials against Registrations table
            var user = _context.Registrations
                .FirstOrDefault(r => r.UserName == loginDto.UserNameOrEmail
                                  && r.Password == loginDto.Password);

            if (user == null)
                return Unauthorized(new { error = "Invalid authentication parameters provided." });

            // claims embedded in the token (identity + role)
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!);

            // build signed JWT with issuer/audience/expiry from config
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaims),
                Issuer = _config["JwtSettings:Issuer"],
                Audience = _config["JwtSettings:Audience"],
                Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_config["JwtSettings:ExpireDays"])),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var createdToken = tokenHandler.CreateToken(tokenDescriptor);
            var bearerToken = $"Bearer {tokenHandler.WriteToken(createdToken)}";

            // persist token to appsettings.json for later retrieval
            SaveTokenToAppSettings(bearerToken, tokenDescriptor.Expires);

            return Ok(new
            {
                token = bearerToken,
                expiration = tokenDescriptor.Expires,
                role = user.Role
            });
        }

        [HttpGet("gettoken")]
        public IActionResult GetToken()
        {
            // read stored token via IConfiguration (DI, no hardcoding)
            var token = _config["GeneratedToken:Value"];
            var expiration = _config["GeneratedToken:Expiration"];

            if (string.IsNullOrWhiteSpace(token))
                return NotFound(new { message = "No token has been generated yet." });

            return Ok(new { token, expiration });
        }

        private void SaveTokenToAppSettings(string bearerToken, DateTime? expiration)
        {
            try
            {
                var filePath = Path.Combine(_env.ContentRootPath, "appsettings.json");
                Console.WriteLine("Writing token to: " + filePath); // temporary debug line

                var json = System.IO.File.ReadAllText(filePath);
                var jsonObj = JsonNode.Parse(json)!.AsObject();

                jsonObj["GeneratedToken"] = new JsonObject
                {
                    ["Value"] = bearerToken,
                    ["Expiration"] = expiration
                };

                var options = new JsonSerializerOptions { WriteIndented = true };
                System.IO.File.WriteAllText(filePath, jsonObj.ToJsonString(options));

                Console.WriteLine("Token saved successfully."); // temporary debug line
            }
            catch (Exception ex)
            {
                Console.WriteLine("FAILED TO SAVE TOKEN: " + ex.Message); // temporary debug line
            }
        }
    }
}