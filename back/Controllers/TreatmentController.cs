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
    [Route("api/treatments")]
    [Authorize]
    public class TreatmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TreatmentController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TreatmentDto>>> Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var entities = await _context.Treatments.Where(c => c.UserId == userId).ToListAsync();
            return Ok(_mapper.Map<List<TreatmentDto>>(entities));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TreatmentDto>> Get(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var entity = await _context.Treatments.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            if (entity == null) return NotFound();
            return Ok(_mapper.Map<TreatmentDto>(entity));
        }

        [HttpPost]
        public async Task<ActionResult<TreatmentDto>> Post(TreatmentDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var entity = _mapper.Map<Treatment>(dto);
            entity.UserId = userId;
            entity.CreatedDate = System.DateTime.UtcNow;
            _context.Treatments.Add(entity);
            await _context.SaveChangesAsync();
            var result = _mapper.Map<TreatmentDto>(entity);
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, TreatmentDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id != dto.Id) return BadRequest();
            var entity = await _context.Treatments.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
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
            var entity = await _context.Treatments.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            if (entity == null) return NotFound();
            _context.Treatments.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
