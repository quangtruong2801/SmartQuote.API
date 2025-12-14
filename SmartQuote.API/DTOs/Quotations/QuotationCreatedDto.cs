
    public class QuotationCreatedDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
