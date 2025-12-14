using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartQuote.API.Data;
using SmartQuote.API.Entities;

namespace SmartQuote.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MaterialsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MaterialsController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/materials (Lấy danh sách vật tư)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Material>>> GetMaterials()
        {
            return await _context.Materials.ToListAsync();
        }

        // 2. POST: api/materials (Thêm vật tư mới)
        [HttpPost]
        public async Task<ActionResult<Material>> CreateMaterial(Material material)
        {
            _context.Materials.Add(material);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMaterials), new { id = material.Id }, material);
        }

        // 3. DELETE: api/materials/5 (Xóa vật tư)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterial(int id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material == null) return NotFound();

            _context.Materials.Remove(material);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 4. PUT: api/materials/5 (Cập nhật vật tư)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaterial(int id, Material material)
        {
            if (id != material.Id) return BadRequest();

            _context.Entry(material).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Materials.Any(e => e.Id == id)) return NotFound();
                throw;
            }

            return NoContent();
        }
    }
}