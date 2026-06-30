using ApplicationStudentManagement.DTOs;
using ApplicationStudentManagement.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DiaSymReader;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Student_management_system.Controllers {
    
    [Route("api/[controller]")]
    public class TokenController : Controller {
        private readonly IRegistrationService _registrationService;
        private readonly IConfiguration _config;

        
        public TokenController(IRegistrationService registrationService, IConfiguration config)
        {
            _registrationService = registrationService;
            _config = config;
        }
    
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateToken([FromBody] LoginViewModel loginViewModel) 
        {
            try
            {
                var user = await _registrationService.CheckAUthenticationAsync(loginViewModel.Email, loginViewModel.Password);
                if (user == null)
                    return Unauthorized(new { error = "Invalid email or password." });

                //Build claims including role
                var authClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["JwtSettings:Secret"]!);
                var tokenDescriptor = new SecurityTokenDescriptor { 
                        Subject = new ClaimsIdentity(authClaims), 
                        Expires = DateTime.UtcNow.AddMinutes(15), 
                        Issuer = _config["JwtSettings:Issuer"], 
                        Audience = _config["JwtSettings:Audience"], 
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) 
                };

                var createdToken = tokenHandler.CreateToken(tokenDescriptor); 
                return Ok(new 
                { 
                    token = tokenHandler.WriteToken(createdToken), 
                    expiration = tokenDescriptor.Expires 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: TokenController
        public ActionResult Index()
        {
            return View();
        }

        // GET: TokenController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TokenController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TokenController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TokenController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TokenController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TokenController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TokenController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
