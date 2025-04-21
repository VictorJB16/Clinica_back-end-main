using AutoMapper;
using Clinica_back_end.DTO.Cita;
using Clinica_back_end.Entities;
using Clinica_back_end.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Clinica_back_end.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("/api/citas")]
    public class CitaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMailService _mailService;

        public CitaController(ApplicationDbContext context,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMailService mailService)
        {
            this._context = context;
            this._mapper = mapper;
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._mailService = mailService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CitaDTO>>> GetAllAsync()
        {
            // verificar si el usuario posee el rol de administrador para obtener todos los roles.
            var userEmailClaim = HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Email).FirstOrDefault();

            if (userEmailClaim == null)
            {
                return BadRequest(new
                {
                    OK = false,
                    message = "No existe una sesión de usuario"
                });
            }

            var userByEmail = await _userManager.FindByEmailAsync(userEmailClaim.Value);

            if (userByEmail == null)
            {
                return BadRequest(new
                {
                    OK = false,
                    message = "No existe una sesión de usuario"
                });
            }

            var userRoles = await _userManager.GetRolesAsync(userByEmail);
            var isAdmin = userRoles.Contains("Admin");

            if (!isAdmin)
            {
                return BadRequest(new
                {
                    OK = false,
                    message = "No cuenta con los permisos necesarios para obtener todos las citas."
                });
            }

            var citas = await _context.Citas.ToListAsync();
            var citasDTO = _mapper.Map<List<CitaDTO>>(citas);

            return citasDTO;
        }

        [HttpGet("user")]
        public async Task<ActionResult<List<CitaDTO>>> GetByUserAsync()
        {
            // verificar si el usuario posee el rol de administrador para obtener todos los roles.
            var userEmailClaim = HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Email).FirstOrDefault();
            if (userEmailClaim == null)
            {
                return BadRequest(new
                {
                    OK = false,
                    message = "No existe una sesión de usuario"
                });
            }

            var userByEmail = await _userManager.FindByEmailAsync(userEmailClaim.Value);
            if (userByEmail == null)
            {
                return BadRequest(new
                {
                    OK = false,
                    message = "No existe el usuario"
                });
            }

            var paciente = await _context.Pacientes.FirstOrDefaultAsync(x => x.ApplicationUserId == userByEmail.Id);
            if (paciente == null)
            {
                return BadRequest(new
                {
                    OK = false,
                    message = "El usuario no esta registrado como paciente"
                });
            }

            var citas = await _context.Citas.Where(x => x.PacienteId == paciente.PacienteId && x.Status == "ACEPTADA").ToListAsync();
            var citasDTO = _mapper.Map<List<CitaDTO>>(citas);

            return citasDTO;
        }


        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] CreateCitaDTO createCitaDTO)
        {
            var userEmail = HttpContext.User.Claims.Where(claim => claim.Type == ClaimTypes.Email).FirstOrDefault();
            var user = await _userManager.FindByNameAsync(userEmail?.Value ?? "");

            if (user == null)
            {
                return BadRequest(new
                {
                    Ok = false,
                    message = "No hay ningúna sesión de usuario"
                });
            }

            // Buscar el paciente en cuestión..
            var paciente = await _context.Pacientes.FirstOrDefaultAsync(x => x.ApplicationUserId == user.Id);
            if (paciente == null)
            {
                return BadRequest(new
                {
                    Ok = false,
                    message = "No existe ningún paciente registrado para este usuario"
                });
            }

            // Verificar si ya existe una cita en el mismo día
            var fechaInicio = createCitaDTO.FechaHora.Date;
            var fechaFin = fechaInicio.AddDays(1);

            var citaExistente = await _context.Citas
                .AnyAsync(x => x.PacienteId == paciente.PacienteId && x.FechaHora >= fechaInicio && x.FechaHora < fechaFin && x.Status == "ACEPTADA");

            if (citaExistente)
            {
                return BadRequest(new
                {
                    Ok = false,
                    message = "Ya tienes una cita registrada para este día"
                });
            }

            var cita = _mapper.Map<Cita>(createCitaDTO);
            cita.PacienteId = paciente.PacienteId;
            cita.Lugar = "Borrar este campo xd";
            cita.Status = "ACEPTADA";

            _context.Add(cita);

            await _context.SaveChangesAsync();

            var sucursal = await _context.Sucursales.FirstOrDefaultAsync(x => x.SucursalId == cita.SucursalId);

            _mailService.SendMail(_mailService.MakeBodyType1(user.UserName!, user.FullName, cita.FechaHora.ToString(), cita.Status, sucursal!.Nombre), "Cita agendada", user.Email!, "SGC");

            return Ok();
        }

        [HttpPost("{id:int}")]
        public async Task<ActionResult> ReactiveAsync([FromRoute] int id)
        {
            var userEmailClaim = HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Email).FirstOrDefault();

            if (userEmailClaim == null)
            {
                return BadRequest(new
                {
                    OK = false,
                    message = "No existe una sesión de usuario"
                });
            }

            var userByEmail = await _userManager.FindByEmailAsync(userEmailClaim.Value);

            if (userByEmail == null)
            {
                return BadRequest(new
                {
                    OK = false,
                    message = "No existe una sesión de usuario"
                });
            }

            var userRoles = await _userManager.GetRolesAsync(userByEmail);
            var isAdmin = userRoles.Contains("Admin");

            if (!isAdmin)
            {
                return BadRequest(new
                {
                    OK = false,
                    message = "No cuenta con los permisos necesarios para obtener todos las citas."
                });
            }

            var citaDb = await _context.Citas.FirstOrDefaultAsync(x => x.CitaId == id);

            if (citaDb == null) return BadRequest(new
            {
                message = "La cita que solicito no existe",
                OK = false
            });

            citaDb.Status = "ACEPTADA";
            _context.Citas.Update(citaDb);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAsync([FromRoute] int id)
        {
            var userEmail = HttpContext.User.Claims.Where(claim => claim.Type == ClaimTypes.Email).FirstOrDefault();
            var user = await _userManager.FindByNameAsync(userEmail?.Value ?? "");

            if (user == null)
            {
                return BadRequest(new
                {
                    Ok = false,
                    message = "No hay ningúna sesión de usuario"
                });
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var isAdmin = userRoles.Contains("Admin");

            var citaDb = await _context.Citas.FirstOrDefaultAsync(x => x.CitaId == id);

            if (citaDb == null) return BadRequest(new
            {
                message = "La cita que solicito no existe",
                OK = false
            });

            // Si es administrador puede borrarla totalmente
            if (isAdmin)
            {

                _context.Remove(citaDb);
                await _context.SaveChangesAsync();

                return Ok();
            }

            // Si no es admin..verificar la condición de 24hrs
            var tiempoRestante = citaDb.FechaHora - DateTime.Now;
            if (tiempoRestante < TimeSpan.FromHours(24))
            {
                return BadRequest(new
                {
                    message = "Las citas solo se pueden cancelar con al menos 24 horas de antelación",
                    Ok = false
                });
            }

            citaDb.Status = "CANCELADA"; // Los usuarios no eliminan, solo cancelan
            _context.Citas.Update(citaDb);
            await _context.SaveChangesAsync();

            return Ok();


        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateAsync([FromRoute] int id, [FromBody] UpdateCitaDTO updateCitaDTO)
        {
            var citaExiste = await _context.Citas.AnyAsync(x => x.CitaId == id);
            if (!citaExiste) return BadRequest(new
            {
                message = "La cita que solicito no existe",
                OK = false
            });

            var cita = _mapper.Map<Cita>(updateCitaDTO);

            cita.CitaId = id;
            _context.Update(cita);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
