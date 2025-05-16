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
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TransactionsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var entities = await _context.Transactions.Where(t => t.UserId == userId).ToListAsync();
            return Ok(_mapper.Map<List<TransactionDto>>(entities));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDto>> Get(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var entity = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (entity == null) return NotFound();
            return Ok(_mapper.Map<TransactionDto>(entity));
        }

        [HttpPost]
        public async Task<ActionResult<TransactionDto>> Post(TransactionDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var entity = _mapper.Map<Transaction>(dto);
            entity.UserId = userId;
            _context.Transactions.Add(entity);
            await _context.SaveChangesAsync();
            var result = _mapper.Map<TransactionDto>(entity);
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, TransactionDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id != dto.Id) return BadRequest();
            var entity = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
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
            var entity = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (entity == null) return NotFound();
            _context.Transactions.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
