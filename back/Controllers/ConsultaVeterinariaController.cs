using System;
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

namespace back.Controllers
{
    [ApiController]
    [Route("api/consultas")]
    [Authorize]
    public class ConsultaVeterinariaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ConsultaVeterinariaController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConsultaVeterinariaDto>>> ObtenerTodas(
            [FromQuery] string? busqueda = null,
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null,
            [FromQuery] int? especieId = null)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var query = _context.ConsultasVeterinarias
                .Include(c => c.EspecieAnimal)
                .Include(c => c.Tratamiento)
                .Where(c => c.UsuarioId == usuarioId);
                
            if (!string.IsNullOrEmpty(busqueda))
            {
                query = query.Where(c => 
                    c.NombreMascota.Contains(busqueda) || 
                    c.NombrePropietario.Contains(busqueda) ||
                    c.Descripcion.Contains(busqueda));
            }
            
            if (fechaInicio.HasValue)
            {
                query = query.Where(c => c.FechaConsulta >= fechaInicio.Value);
            }
            
            if (fechaFin.HasValue)
            {
                query = query.Where(c => c.FechaConsulta <= fechaFin.Value);
            }
            
            if (especieId.HasValue)
            {
                query = query.Where(c => c.EspecieAnimalId == especieId.Value);
            }
            
            var consultas = await query.ToListAsync();
            var dtos = _mapper.Map<List<ConsultaVeterinariaDto>>(consultas);
            
            // Agregar información para UI
            foreach (var dto in dtos)
            {
                var consulta = consultas.First(c => c.Id == dto.Id);
                dto.NombreEspecieAnimal = consulta.EspecieAnimal?.Nombre;
                dto.NombreTratamiento = consulta.Tratamiento?.Nombre;
            }
            
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ConsultaVeterinariaDto>> ObtenerPorId(int id)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var consulta = await _context.ConsultasVeterinarias
                .Include(c => c.EspecieAnimal)
                .Include(c => c.Tratamiento)
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);
                
            if (consulta == null)
                return NotFound(new { Mensaje = "Consulta veterinaria no encontrada" });
                
            var dto = _mapper.Map<ConsultaVeterinariaDto>(consulta);
            dto.NombreEspecieAnimal = consulta.EspecieAnimal?.Nombre;
            dto.NombreTratamiento = consulta.Tratamiento?.Nombre;
                
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<ConsultaVeterinariaDto>> Crear(ConsultaVeterinariaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            // Verificar que la especie animal existe
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var especieAnimal = await _context.EspeciesAnimales
                .FirstOrDefaultAsync(e => e.Id == dto.EspecieAnimalId && e.UsuarioId == usuarioId);
                
            if (especieAnimal == null)
                return BadRequest(new { Mensaje = "La especie animal especificada no existe" });
                
            // Verificar que el tratamiento existe
            var tratamiento = await _context.Tratamientos
                .FirstOrDefaultAsync(t => t.Id == dto.TratamientoId && t.UsuarioId == usuarioId);
                
            if (tratamiento == null)
                return BadRequest(new { Mensaje = "El tratamiento especificado no existe" });
                
            var consulta = _mapper.Map<ConsultaVeterinaria>(dto);
            consulta.UsuarioId = usuarioId;
            
            _context.ConsultasVeterinarias.Add(consulta);
            await _context.SaveChangesAsync();
            
            // Cargar relaciones para devolver datos completos
            await _context.Entry(consulta).Reference(c => c.EspecieAnimal).LoadAsync();
            await _context.Entry(consulta).Reference(c => c.Tratamiento).LoadAsync();
            
            var resultado = _mapper.Map<ConsultaVeterinariaDto>(consulta);
            resultado.NombreEspecieAnimal = consulta.EspecieAnimal?.Nombre;
            resultado.NombreTratamiento = consulta.Tratamiento?.Nombre;
            
            return CreatedAtAction(
                nameof(ObtenerPorId), 
                new { id = consulta.Id }, 
                resultado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, ConsultaVeterinariaDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { Mensaje = "Los IDs no coinciden" });
                
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Verificar existencia de la consulta
            var consulta = await _context.ConsultasVeterinarias
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);
                
            if (consulta == null)
                return NotFound(new { Mensaje = "Consulta veterinaria no encontrada" });
                
            // Verificar existencia de especie animal
            var especieAnimal = await _context.EspeciesAnimales
                .FirstOrDefaultAsync(e => e.Id == dto.EspecieAnimalId && e.UsuarioId == usuarioId);
                
            if (especieAnimal == null)
                return BadRequest(new { Mensaje = "La especie animal especificada no existe" });
                
            // Verificar existencia de tratamiento
            var tratamiento = await _context.Tratamientos
                .FirstOrDefaultAsync(t => t.Id == dto.TratamientoId && t.UsuarioId == usuarioId);
                
            if (tratamiento == null)
                return BadRequest(new { Mensaje = "El tratamiento especificado no existe" });
                
            _mapper.Map(dto, consulta);
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConsultaVeterinariaExiste(id))
                    return NotFound();
                throw;
            }
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var consulta = await _context.ConsultasVeterinarias
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);
                
            if (consulta == null)
                return NotFound(new { Mensaje = "Consulta veterinaria no encontrada" });
                
            _context.ConsultasVeterinarias.Remove(consulta);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        private bool ConsultaVeterinariaExiste(int id)
        {
            return _context.ConsultasVeterinarias.Any(c => c.Id == id);
        }
    }
}
