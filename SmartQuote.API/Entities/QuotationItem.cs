using System.ComponentModel.DataAnnotations;

namespace SmartQuote.API.Entities
{
    public class QuotationItem
    {
        public int Id { get; set; }

        // FK
        public int QuotationId { get; set; }

        public Quotation Quotation { get; set; } = null!;

        // Snapshot sản phẩm
        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        // Kích thước
        [Range(0, double.MaxValue)]
        public double Width { get; set; }

        [Range(0, double.MaxValue)]
        public double Height { get; set; }

        [Range(0, double.MaxValue)]
        public double Depth { get; set; }

        // Snapshot vật tư
        public int MaterialId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal UnitPriceSnapshot { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        // Thành tiền
        [Range(0, double.MaxValue)]
        public decimal TotalPrice { get; set; }
    }
}
