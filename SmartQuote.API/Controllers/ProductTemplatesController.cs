using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
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
        private readonly Cloudinary _cloudinary;

        public ProductTemplatesController(
            AppDbContext context,
            IConfiguration config)
        {
            _context = context;

            var account = new Account(
                config["Cloudinary:CloudName"],
                config["Cloudinary:ApiKey"],
                config["Cloudinary:ApiSecret"]
            );

            _cloudinary = new Cloudinary(account);
        }

        // GET: api/ProductTemplates
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductTemplate>>> GetProductTemplates()
        {
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
            var materialExists = await _context.Materials
                .AnyAsync(m => m.Id == productTemplate.DefaultMaterialId);

            if (!materialExists)
                return BadRequest("Vật tư mặc định không tồn tại.");

            _context.ProductTemplates.Add(productTemplate);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetProductTemplate),
                new { id = productTemplate.Id },
                productTemplate
            );
        }

        // PUT: api/ProductTemplates/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductTemplate(int id, ProductTemplate updated)
        {
            if (id != updated.Id) return BadRequest();

            var existing = await _context.ProductTemplates.FindAsync(id);
            if (existing == null) return NotFound();

            // 🔥 Nếu đổi ảnh → xóa ảnh cũ
            if (!string.IsNullOrEmpty(existing.ImagePublicId) &&
                existing.ImagePublicId != updated.ImagePublicId)
            {
                try
                {
                    await _cloudinary.DestroyAsync(
                        new DeletionParams(existing.ImagePublicId));
                }
                catch
                {
                    // log nếu cần, không throw
                }
            }

            existing.Name = updated.Name;
            existing.ImageUrl = updated.ImageUrl;
            existing.ImagePublicId = updated.ImagePublicId;
            existing.DefaultMaterialId = updated.DefaultMaterialId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/ProductTemplates/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductTemplate(int id)
        {
            var productTemplate = await _context.ProductTemplates.FindAsync(id);
            if (productTemplate == null) return NotFound();

            // 🔥 Xóa ảnh Cloudinary
            if (!string.IsNullOrEmpty(productTemplate.ImagePublicId))
            {
                try
                {
                    await _cloudinary.DestroyAsync(
                        new DeletionParams(productTemplate.ImagePublicId));
                }
                catch
                {
                    // log nếu cần
                }
            }

            _context.ProductTemplates.Remove(productTemplate);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
