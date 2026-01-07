using System.ComponentModel.DataAnnotations;

namespace SmartQuote.API.Entities
{
    public class Quotation
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public QuotationStatus Status { get; set; } = QuotationStatus.Draft;

        public double DiscountPercent { get; set; } = 0;
        public double TaxPercent { get; set; } = 0;

        // Tổng tiền (tính từ Items)
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        // Khách hàng
        public int CustomerId { get; set; }

        public Customer Customer { get; set; } = null!;

        // Danh sách sản phẩm
        public List<QuotationItem> Items { get; set; } = new();
    }
}
