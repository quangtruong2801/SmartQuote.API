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
    public class ProductTemplatesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductTemplatesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ProductTemplates
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductTemplate>>> GetProductTemplates()
        {
            // Include(p => p.DefaultMaterial): Lấy luôn thông tin tên vật tư đi kèm
            return await _context.ProductTemplates
                .Include(p => p.DefaultMaterial)
                .ToListAsync();
        }

        // GET: api/ProductTemplates/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductTemplate>> GetProductTemplate(int id)
        {
            var productTemplate = await _context.ProductTemplates
                .Include(p => p.DefaultMaterial)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (productTemplate == null) return NotFound();
            return productTemplate;
        }

        // POST: api/ProductTemplates
        [HttpPost]
        public async Task<ActionResult<ProductTemplate>> CreateProductTemplate(ProductTemplate productTemplate)
        {
            // Kiểm tra xem MaterialId có tồn tại không
            var materialExists = await _context.Materials.AnyAsync(m => m.Id == productTemplate.DefaultMaterialId);
            if (!materialExists) return BadRequest("Vật tư mặc định không tồn tại!");

            _context.ProductTemplates.Add(productTemplate);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductTemplate), new { id = productTemplate.Id }, productTemplate);
        }

        // PUT: api/ProductTemplates/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductTemplate(int id, ProductTemplate productTemplate)
        {
            if (id != productTemplate.Id) return BadRequest();

            _context.Entry(productTemplate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ProductTemplates.Any(e => e.Id == id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/ProductTemplates/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductTemplate(int id)
        {
            var productTemplate = await _context.ProductTemplates.FindAsync(id);
            if (productTemplate == null) return NotFound();

            _context.ProductTemplates.Remove(productTemplate);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}