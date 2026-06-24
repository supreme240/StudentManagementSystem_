using ApplicationStudentManagement.DTO;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using StudentManagement.domain.Domain;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Student_management_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : Controller
    {
        // Connection string from appsettings.json
        private readonly string _connectionString;

        // Secret key to sign the JWT token
        private readonly string _jwtKey = "YourSuperSecretKeyHere123456789";

        // Who created the token
        private readonly string _issuer = "StudentManagementSystem";

        // Who the token is for
        private readonly string _audience = "StudentManagementSystem";

        // Inject IConfiguration to read connection string
        public TokenController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // Creates SQL connection — same pattern as DapperRegistrationRepository
        private IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateToken([FromBody] LogInViewModel model)
        {
            // Validate [Required] fields from LogInViewModel
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var db = CreateConnection();

            // Check both Username and Email columns against UserNameOrEmail
            // Replace "Users" with your actual table name
            const string sql = @"SELECT * FROM Users 
                                 WHERE (Username = @UserNameOrEmail 
                                 OR Email = @UserNameOrEmail) 
                                 AND Password = @Password";

            // Returns null if user not found
            var user = await db.QueryFirstOrDefaultAsync(sql, new
            {
                model.UserNameOrEmail,
                model.Password
            });

            // Return 401 if credentials don't match
            if (user == null)
                return Unauthorized("Invalid username/email or password");

            // Generate and return the JWT token
            var token = CreateToken(model.UserNameOrEmail, "Student");
            return Ok(new { token });
        }

        private string CreateToken(string usernameOrEmail, string role)
        {
            // User info stored inside the token
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usernameOrEmail),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Sign the token with secret key using HMAC SHA256
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Build the token
            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60), // expires in 60 mins
                signingCredentials: creds
            );

            // Convert token to string and return
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}