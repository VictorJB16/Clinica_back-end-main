using AutoMapper;
using Clinica_back_end.DTO.Cita;
using Clinica_back_end.DTO.TipoCita;
using Clinica_back_end.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinica_back_end.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("/api/tipo-cita")]
    public class TipoCitaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TipoCitaController(ApplicationDbContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<TipoCitaDTO>>> GetAllAsync()
        {
            var citasDb = await _context.TiposCita.ToListAsync();
            var citas = _mapper.Map<List<TipoCita>>(citasDb);

            return Ok(citas);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] CreateTipoCitaDTO createTipoCitaDTO)
        {
            var tipoExiste = await _context.TiposCita.AnyAsync(x => x.Nombre.ToLower().Trim() == createTipoCitaDTO.Nombre.ToLower().Trim());

            if (tipoExiste) return BadRequest(new
            {
                message = "El tipo de cita indicado, ya existe",
                OK = false,
            });

            var tipo = _mapper.Map<TipoCita>(createTipoCitaDTO);

            _context.TiposCita.Add(tipo);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAsync([FromRoute] int id)
        {
            var tipoExiste = await _context.TiposCita.FirstOrDefaultAsync(x => x.TipoCitaId == id);

            if (tipoExiste == null) return BadRequest(new
            {
                message = "La cita solicitada no existe",
                OK = false,
            });

            _context.TiposCita.Remove(tipoExiste);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateAsync([FromRoute] int id, [FromBody] UpdateTipoCitaDTO updateTipoCitaDTO)
        {
            var tipoExiste = await _context.TiposCita.AnyAsync(x => x.TipoCitaId == id);

            if (!tipoExiste) return BadRequest(new
            {
                message = "La cita solicitada no existe",
                OK = false,
            });

            var tipoCita = _mapper.Map<TipoCita>(updateTipoCitaDTO);

            tipoCita.TipoCitaId = id;
            _context.TiposCita.Update(tipoCita);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
