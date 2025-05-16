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
    [Route("api/healthplans")]
    [Authorize]
    public class HealthPlanController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public HealthPlanController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HealthPlanDto>>> Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var plans = await _context.HealthPlans
                .Include(b => b.Treatment)
                .Where(b => b.UserId == userId)
                .ToListAsync();
            
            var result = _mapper.Map<List<HealthPlanDto>>(plans);
            
            // Añadir nombres de tratamientos para UI
            foreach (var dto in result)
            {
                var plan = plans.First(p => p.Id == dto.Id);
                dto.TreatmentName = plan.Treatment?.Name;
            }
            
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HealthPlanDto>> Get(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var plan = await _context.HealthPlans
                .Include(b => b.Treatment)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
                
            if (plan == null) return NotFound();
            
            var result = _mapper.Map<HealthPlanDto>(plan);
            result.TreatmentName = plan.Treatment?.Name;
            
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<HealthPlanDto>> Post(HealthPlanDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var entity = _mapper.Map<HealthPlan>(dto);
            entity.UserId = userId;
            
            _context.HealthPlans.Add(entity);
            await _context.SaveChangesAsync();
            
            // Cargar el tratamiento relacionado para el resultado
            await _context.Entry(entity)
                .Reference(b => b.Treatment)
                .LoadAsync();
                
            var result = _mapper.Map<HealthPlanDto>(entity);
            result.TreatmentName = entity.Treatment?.Name;
            
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, HealthPlanDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id != dto.Id) return BadRequest();
            
            var entity = await _context.HealthPlans
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
                
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
            var entity = await _context.HealthPlans
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
                
            if (entity == null) return NotFound();
            
            _context.HealthPlans.Remove(entity);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
    }
}
