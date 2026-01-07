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
    public class QuotationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuotationsController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET: api/Quotations
        // =========================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuotationListDto>>> GetQuotations()
        {
            var quotations = await _context.Quotations
                .Include(q => q.Customer)
                .OrderByDescending(q => q.CreatedAt)
                .Select(q => new QuotationListDto
                {
                    Id = q.Id,
                    CustomerId = q.CustomerId,
                    CustomerName = q.Customer.Name,
                    Status = q.Status.ToString(),
                    TotalAmount = q.TotalAmount,
                    CreatedAt = q.CreatedAt
                })
                .ToListAsync();

            return Ok(quotations);
        }

        // =========================
        // GET: api/Quotations/{id}
        // (Chi tiết – in PDF)
        // =========================
        [HttpGet("{id}")]
        public async Task<ActionResult<QuotationDetailDto>> GetQuotation(int id)
        {
            var quotation = await _context.Quotations
                .Include(q => q.Customer)
                .Include(q => q.Items)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quotation == null)
                return NotFound();

            var result = new QuotationDetailDto
            {
                Id = quotation.Id,
                CustomerId = quotation.CustomerId,
                CustomerName = quotation.Customer.Name,
                CustomerPhone = quotation.Customer?.Phone ?? "",
                CustomerAddress = quotation.Customer?.Address ?? "",
                CustomerEmail = quotation.Customer?.Email ?? "",
                Status = quotation.Status.ToString(),
                TotalAmount = quotation.TotalAmount,
                DiscountPercent = quotation.DiscountPercent,
                TaxPercent = quotation.TaxPercent,
                CreatedAt = quotation.CreatedAt,
                Items = quotation.Items.Select(i => new QuotationItemResponseDto
                {
                    ProductName = i.ProductName,
                    Width = i.Width,
                    Height = i.Height,
                    Depth = i.Depth,
                    MaterialId = i.MaterialId,
                    UnitPriceSnapshot = i.UnitPriceSnapshot,
                    Quantity = i.Quantity,
                    TotalPrice = i.TotalPrice
                }).ToList()
            };

            return Ok(result);
        }

        // PUT: api/Quotations/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto request) // <--- Sửa tham số này
        {
            var quotation = await _context.Quotations.FindAsync(id);
            if (quotation == null) return NotFound();

            // Lấy status từ object request
            quotation.Status = (QuotationStatus)request.Status;

            if (quotation.Status == QuotationStatus.Approved || quotation.Status == QuotationStatus.Rejected)
            {
                quotation.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật trạng thái thành công" });
        }


        // =========================
        // POST: api/Quotations
        // Tạo báo giá mới
        // =========================
        [HttpPost]
        public async Task<ActionResult<QuotationCreatedDto>> CreateQuotation(QuotationCreateDto request)
        {
            if (request.Items == null || !request.Items.Any())
                return BadRequest("Quotation must have at least one item");

            var quotation = new Quotation
            {
                CustomerId = request.CustomerId,
                Status = QuotationStatus.Draft,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                DiscountPercent = request.DiscountPercent,
                TaxPercent = request.TaxPercent,
                Items = new List<QuotationItem>()
            };

            decimal subTotal = 0;

            foreach (var itemDto in request.Items)
            {
                var material = await _context.Materials.FindAsync(itemDto.MaterialId);
                if (material == null)
                    return BadRequest($"Vật tư ID {itemDto.MaterialId} không tồn tại");

                var itemTotal = itemDto.UnitPriceSnapshot * itemDto.Quantity;
                subTotal += itemTotal;

                quotation.Items.Add(new QuotationItem
                {
                    ProductName = itemDto.ProductName,
                    Width = itemDto.Width,
                    Height = itemDto.Height,
                    Depth = itemDto.Depth,
                    MaterialId = itemDto.MaterialId,
                    UnitPriceSnapshot = itemDto.UnitPriceSnapshot,
                    Quantity = itemDto.Quantity,
                    TotalPrice = itemTotal
                });
            }

            // Tính tiền giảm giá
            decimal discountAmount = subTotal * (decimal)(request.DiscountPercent / 100.0);
            decimal afterDiscount = subTotal - discountAmount;

            // Tính tiền thuế (Dựa trên số đã chiết khấu)
            decimal taxAmount = afterDiscount * (decimal)(request.TaxPercent / 100.0);

            // Tổng cuối cùng
            quotation.TotalAmount = afterDiscount + taxAmount;

            _context.Quotations.Add(quotation);
            await _context.SaveChangesAsync();

            var response = new QuotationCreatedDto
            {
                Id = quotation.Id,
                CustomerId = quotation.CustomerId,
                Status = quotation.Status.ToString(),
                TotalAmount = quotation.TotalAmount,
                CreatedAt = quotation.CreatedAt
            };

            return CreatedAtAction(nameof(GetQuotation), new { id = quotation.Id }, response);
        }
    }
}
