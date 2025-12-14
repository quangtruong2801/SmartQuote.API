using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartQuote.API.Data;
using SmartQuote.API.Entities;
namespace SmartQuote.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
        {
            // --- CÁC SỐ LIỆU TỔNG QUAN (GIỮ NGUYÊN) ---
            // Lưu ý: Doanh thu nên chỉ tính đơn đã Approved (đã chốt)
            var totalRevenue = await _context.Quotations
                .Where(q => q.Status == QuotationStatus.Approved)
                .SumAsync(q => q.TotalAmount);

            var totalQuotations = await _context.Quotations.CountAsync();
            var totalCustomers = await _context.Customers.CountAsync();
            var totalProducts = await _context.ProductTemplates.CountAsync();

            // --- TÍNH TOÁN BIỂU ĐỒ (7 NGÀY GẦN NHẤT) ---
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7).Date;

            var chartData = await _context.Quotations
                // Chỉ lấy đơn đã duyệt & trong 7 ngày qua
                .Where(q => q.Status == QuotationStatus.Approved && q.CreatedAt >= sevenDaysAgo)
                .GroupBy(q => q.CreatedAt.Date) // Nhóm theo ngày
                .Select(g => new ChartDataDto
                {
                    Date = g.Key,
                    Revenue = g.Sum(q => q.TotalAmount)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return new DashboardSummaryDto
            {
                TotalRevenue = totalRevenue,
                TotalQuotations = totalQuotations,
                TotalCustomers = totalCustomers,
                TotalProducts = totalProducts,
                ChartData = chartData 
            };
        }
    }
}