using Clinica_back_end.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinica_back_end.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("/api/setup")]
    public class SetupController(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager) : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;

        [HttpGet("roles")]
        public async Task<ActionResult<List<IdentityRole>>> GetRoles()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> CreateRole([FromBody] string roleName)
        {
            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (roleExist)
            {
                return BadRequest(new
                {
                    OK = false,
                    message = $"El rol {roleName} ya existe"
                });
            }

            var rolResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (!rolResult.Succeeded)
            {
                return BadRequest(new
                {
                    OK = false,
                    message = $"El rol {roleName} no se pudo agregar"
                });
            }

            return Ok();
        }

        [HttpGet("getAllUsers")]
        public async Task<ActionResult> GetAllUsers()
        {
            return Ok(await _userManager.Users.ToListAsync());
        }

        [HttpPost("AddUserToRole")]
        public async Task<ActionResult> AddUserToRole(string email, string roleName)
        {
            var user = await _userManager.FindByNameAsync(email);
            if (user == null)
            {
                return BadRequest(new
                {
                    OK = false,
                    message = $"El usuario {email} no existe"
                });
            }

            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                return BadRequest(new
                {
                    OK = false,
                    message = $"El rol {roleName} no existe"
                });
            }

            var assignResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!assignResult.Succeeded)
            {
                return BadRequest(new
                {
                    OK = false,
                    message = $"No se pudo agregar el rol"
                });
            }

            return Ok();

        }
    }
}
