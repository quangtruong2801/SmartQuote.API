
    public class QuotationCreateDto
    {
        public int CustomerId { get; set; }
        public required List<QuotationItemCreateDto> Items { get; set; }
        public double DiscountPercent { get; set; } = 0;
        public double TaxPercent { get; set; } = 0;
}
