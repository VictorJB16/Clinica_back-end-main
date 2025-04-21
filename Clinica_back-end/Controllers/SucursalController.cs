using AutoMapper;
using Clinica_back_end.DTO.Paciente;
using Clinica_back_end.DTO.Sucursal;
using Clinica_back_end.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinica_back_end.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("/api/sucursales")]
    public class SucursalController: ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SucursalController(ApplicationDbContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<SucursalDTO>>> GetAllAsync()
        {
            var sucursalesDB = await _context.Sucursales.ToListAsync();
            return _mapper.Map<List<SucursalDTO>>(sucursalesDB);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] CreateSucursalDTO createSucursalDTO)
        {
            var sucursal = _mapper.Map<Sucursal>(createSucursalDTO);
            _context.Add(sucursal);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAsync([FromRoute] int id)
        {
            var sucursal_result = await _context.Sucursales.FirstOrDefaultAsync(x => x.SucursalId == id);
            if (sucursal_result == null) return BadRequest(new
            {
                message = "La sucursal que solicito no existe",
                OK = false
            });

            _context.Sucursales.Remove(sucursal_result);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateAsync([FromRoute] int id, [FromBody] UpdateSucursalDTO updateSucursalDTO)
        {
            var sucursal_result = await _context.Sucursales.AnyAsync(x => x.SucursalId == id);
            if (!sucursal_result) return BadRequest(new
            {
                message = "La sucursal que solicito no existe",
                OK = false
            });

            var sucursal = _mapper.Map<Sucursal>(updateSucursalDTO);
            sucursal.SucursalId = id;

            _context.Sucursales.Update(sucursal);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
