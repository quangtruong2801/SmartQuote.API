
    public class QuotationItemCreateDto
    {
        public required string ProductName { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }
        public int MaterialId { get; set; }
        public decimal UnitPriceSnapshot { get; set; }
        public int Quantity { get; set; }
    }
