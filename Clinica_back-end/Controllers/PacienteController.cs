using AutoMapper;
using Clinica_back_end.DTO.Cita;
using Clinica_back_end.DTO.Paciente;
using Clinica_back_end.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinica_back_end.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("/api/pacientes")]
    public class PacienteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PacienteController(ApplicationDbContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<PacienteDTO>>> GetAllAsync()
        {
            var pacientesDB = await _context.Pacientes.ToListAsync();
            return _mapper.Map<List<PacienteDTO>>(pacientesDB);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] CreatePacienteDTO createPacienteDTO)
        {
            var paciente = _mapper.Map<Paciente>(createPacienteDTO);
            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();

            return Ok();
        }
        

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAsync([FromRoute] int id)
        {
            var paciente_result = await _context.Pacientes.FirstOrDefaultAsync(x => x.PacienteId == id);
            if (paciente_result == null) return BadRequest(new
            {
                message = "La cita que solicito no existe",
                OK = false
            });

            _context.Remove(paciente_result);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateAsync([FromRoute] int id, [FromBody] UpdatePacienteDTO updatePacienteDTO)
        {
            var pacienteExiste = await _context.Pacientes.AnyAsync(x => x.PacienteId == id);
            if (!pacienteExiste) return BadRequest(new
            {
                message = "La cita que solicito no existe",
                OK = false
            });

            var paciente = _mapper.Map<Cita>(updatePacienteDTO);
            
            paciente.PacienteId = id;
            _context.Update(paciente);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
