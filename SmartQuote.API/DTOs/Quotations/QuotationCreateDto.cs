
    public class QuotationCreateDto
    {
        public int CustomerId { get; set; }
        public required List<QuotationItemCreateDto> Items { get; set; }
    }
