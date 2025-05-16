using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back.Data;
using back.Models;
using back.DTOs;
using AutoMapper;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System;

namespace back.Controllers
{
    [ApiController]
    [Route("api/especies")]
    [Authorize]
    public class EspecieAnimalController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public EspecieAnimalController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EspecieAnimalDto>>> ObtenerTodos([FromQuery] string? busqueda = null)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var query = _context.EspeciesAnimales
                .Where(e => e.UsuarioId == usuarioId);
                
            if (!string.IsNullOrEmpty(busqueda))
            {
                query = query.Where(e => 
                    e.Nombre.Contains(busqueda) || 
                    e.Descripcion.Contains(busqueda));
            }
            
            var especies = await query.ToListAsync();
            
            var dtos = _mapper.Map<List<EspecieAnimalDto>>(especies);
            
            // Contar consultas para cada especie
            foreach (var dto in dtos)
            {
                dto.NumeroConsultas = await _context.ConsultasVeterinarias
                    .CountAsync(c => c.EspecieAnimalId == dto.Id && c.UsuarioId == usuarioId);
            }
            
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EspecieAnimalDto>> ObtenerPorId(int id)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var especie = await _context.EspeciesAnimales
                .FirstOrDefaultAsync(e => e.Id == id && e.UsuarioId == usuarioId);
                
            if (especie == null)
                return NotFound(new { Mensaje = "Especie animal no encontrada" });
                
            var dto = _mapper.Map<EspecieAnimalDto>(especie);
            
            dto.NumeroConsultas = await _context.ConsultasVeterinarias
                .CountAsync(c => c.EspecieAnimalId == dto.Id && c.UsuarioId == usuarioId);
                
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<EspecieAnimalDto>> Crear(EspecieAnimalDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var especie = _mapper.Map<EspecieAnimal>(dto);
            
            especie.UsuarioId = usuarioId;
            especie.FechaCreacion = DateTime.UtcNow;
            
            _context.EspeciesAnimales.Add(especie);
            await _context.SaveChangesAsync();
            
            dto = _mapper.Map<EspecieAnimalDto>(especie);
            
            return CreatedAtAction(nameof(ObtenerPorId), new { id = especie.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, EspecieAnimalDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { Mensaje = "Los IDs no coinciden" });
                
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var especie = await _context.EspeciesAnimales
                .FirstOrDefaultAsync(e => e.Id == id && e.UsuarioId == usuarioId);
                
            if (especie == null)
                return NotFound(new { Mensaje = "Especie animal no encontrada" });
                
            _mapper.Map(dto, especie);
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EspecieAnimalExiste(id))
                    return NotFound();
                throw;
            }
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var especie = await _context.EspeciesAnimales
                .FirstOrDefaultAsync(e => e.Id == id && e.UsuarioId == usuarioId);
                
            if (especie == null)
                return NotFound(new { Mensaje = "Especie animal no encontrada" });
                
            // Verificar si hay consultas asociadas
            var tieneConsultas = await _context.ConsultasVeterinarias
                .AnyAsync(c => c.EspecieAnimalId == id);
                
            if (tieneConsultas)
                return BadRequest(new { Mensaje = "No se puede eliminar la especie porque tiene consultas asociadas" });
                
            _context.EspeciesAnimales.Remove(especie);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        private bool EspecieAnimalExiste(int id)
        {
            return _context.EspeciesAnimales.Any(e => e.Id == id);
        }
    }
}
