
    public class QuotationItemResponseDto
    {
        public string ProductName { get; set; } = string.Empty;
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }
        public int MaterialId { get; set; }
        public decimal UnitPriceSnapshot { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
