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
    [Route("api/consultations")]
    [Authorize]
    public class VeterinaryConsultationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public VeterinaryConsultationController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VeterinaryConsultationDto>>> Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var consultations = await _context.VeterinaryConsultations
                .Include(t => t.AnimalSpecies)
                .Include(t => t.Treatment)
                .Where(t => t.UserId == userId)
                .ToListAsync();
            
            var result = _mapper.Map<List<VeterinaryConsultationDto>>(consultations);
            
            // Añadir nombres de especies y tratamientos para UI
            foreach (var dto in result)
            {
                var consultation = consultations.First(c => c.Id == dto.Id);
                dto.AnimalSpeciesName = consultation.AnimalSpecies?.Name;
                dto.TreatmentName = consultation.Treatment?.Name;
            }
            
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VeterinaryConsultationDto>> Get(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var consultation = await _context.VeterinaryConsultations
                .Include(t => t.AnimalSpecies)
                .Include(t => t.Treatment)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
                
            if (consultation == null) return NotFound();
            
            var result = _mapper.Map<VeterinaryConsultationDto>(consultation);
            result.AnimalSpeciesName = consultation.AnimalSpecies?.Name;
            result.TreatmentName = consultation.Treatment?.Name;
            
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<VeterinaryConsultationDto>> Post(VeterinaryConsultationDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var entity = _mapper.Map<VeterinaryConsultation>(dto);
            entity.UserId = userId;
            
            _context.VeterinaryConsultations.Add(entity);
            await _context.SaveChangesAsync();
            
            var result = _mapper.Map<VeterinaryConsultationDto>(entity);
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, VeterinaryConsultationDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id != dto.Id) return BadRequest();
            
            var entity = await _context.VeterinaryConsultations
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
                
            if (entity == null) return NotFound();
            
            _mapper.Map(dto, entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var entity = await _context.VeterinaryConsultations
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
                
            if (entity == null) return NotFound();
            
            _context.VeterinaryConsultations.Remove(entity);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
    }
}
