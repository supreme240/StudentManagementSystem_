using ApplicationStudentManagement.DTO;
using ApplicationStudentManagement.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Student_management_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class StudentAPIController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;
        private readonly IRolesService _rolesService;

        public StudentAPIController(
            IRegistrationService registrationService,
            IRolesService rolesService)
        {
            _registrationService = registrationService;
            _rolesService = rolesService;
        }

        // GET: api/StudentAPI/getstudent
        [HttpGet("getstudent")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetStudentInfo()
        {
            try
            {
                var viewModels = await _registrationService.GetAllRegistrationsAsync();
                return Ok(viewModels);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Unable to load the student list. Please try again later." });
            }
        }

        // GET: api/StudentAPI/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var registration = await _registrationService.GetRegistrationByIdAsync(id); 

                if (registration == null)
                    return NotFound(new { message = $"No registration found with id {id}." });

                return Ok(registration);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Unable to load this student's details. Please try again later." });
            }
        }

        // POST: api/StudentAPI
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromBody] RegistrationViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var (success, error) = await _registrationService.AddRegistrationAsync(viewModel);

                if (!success)
                    return BadRequest(new { message = error });

                return Ok(new { message = "Registration successful!" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again." });
            }
        }

        // PUT: api/StudentAPI/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(int id, [FromBody] RegistrationViewModel viewModel)
        {
            if (id != viewModel.Id)
                return BadRequest(new { message = "ID mismatch." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var exists = await _registrationService.GetRegistrationByIdAsync(id);

                if (exists == null)
                    return NotFound(new { message = $"No registration found with id {id}." });

                var (success, error) = await _registrationService.UpdateRegistrationAsync(viewModel);

                if (!success)
                    return BadRequest(new { message = error });

                return Ok(new { message = "Registration updated successfully!" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while updating. Please try again." });
            }
        }

        // DELETE: api/StudentAPI/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(student ctf)
        {
            try
            {
                var exists = await _registrationService.GetRegistrationByIdAsync(id);

                if (exists == null)
                    return NotFound(new { message = $"No registration found with id {id}." });

                await _registrationService.DeleteRegistrationAsync(id);
                return Ok(new { message = "Registration deleted successfully!" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Unable to delete this registration. Please try again later." });
            }
        }
    }
}