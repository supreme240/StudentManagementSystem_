using ApplicationStudentManagement.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Student_management_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class StudentAPIController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;
        private readonly IRolesService _rolesService;

        // Constructor: ASP.NET automatically provides these services
        public StudentAPIController(
            IRegistrationService registrationService,
            IRolesService rolesService)
        {
            _registrationService = registrationService;
            _rolesService = rolesService;
        }
        // GET: api/<StudentAPIController>
        [HttpGet("getstudent")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetStudentInfo()
        {
            var viewModels = await _registrationService.GetAllRegistrationsAsync();
            return Ok(viewModels);
        }

        // GET api/<StudentAPIController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<StudentAPIController>
        [HttpPost]
        public void Post([FromBody] Registrationmodel model)
        {
        }

        // PUT api/<StudentAPIController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<StudentAPIController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
