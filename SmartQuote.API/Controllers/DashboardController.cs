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
        public async Task<ActionResult<DashboardSummaryDto>> GetSummary([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            // 1. Xác định khoảng thời gian (Mặc định 30 ngày nếu không chọn)
            var end = toDate?.Date.AddDays(1).AddTicks(-1) ?? DateTime.UtcNow.Date.AddDays(1).AddTicks(-1); // Cuối ngày
            var start = fromDate?.Date ?? end.AddDays(-30).Date; // Mặc định lùi 30 ngày

            // --- CÁC SỐ LIỆU TỔNG QUAN (STAT CARDS) ---

            // Doanh thu & Đơn hàng (Nên filter theo ngày chọn để số liệu khớp với biểu đồ)
            var revenueQuery = _context.Quotations
                .Where(q => q.Status == QuotationStatus.Approved && q.CreatedAt >= start && q.CreatedAt <= end);

            var totalRevenue = await revenueQuery.SumAsync(q => q.TotalAmount);

            // Tổng đơn hàng (tính trong khoảng thời gian chọn)
            var totalQuotations = await _context.Quotations
                .Where(q => q.CreatedAt >= start && q.CreatedAt <= end)
                .CountAsync();

            // Khách hàng & Sản phẩm (Thường là tính tổng toàn hệ thống, không theo ngày)
            var totalCustomers = await _context.Customers.CountAsync();
            var totalProducts = await _context.ProductTemplates.CountAsync();

            // --- TÍNH TOÁN BIỂU ĐỒ ---
            var chartData = await revenueQuery
                .GroupBy(q => q.CreatedAt.Date)
                .Select(g => new ChartDataDto
                {
                    Date = g.Key,
                    Revenue = g.Sum(q => q.TotalAmount)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            // (Optional) Fill các ngày không có doanh thu bằng 0 để biểu đồ đẹp hơn
            // Logic này có thể xử lý ở Frontend hoặc Backend tùy bạn.

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