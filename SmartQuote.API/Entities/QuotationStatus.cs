namespace SmartQuote.API.Entities
{
    public enum QuotationStatus
    {
        Draft = 0,      // Nháp
        Sent = 1,       // Đã gửi khách
        Approved = 2,   // Khách đồng ý chốt
        Rejected = 3    // Khách từ chối
    }
}