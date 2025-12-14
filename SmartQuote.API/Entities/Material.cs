namespace SmartQuote.API.Entities
{
    public class Material
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Tên vật tư (Gỗ MDF...)
        public string Unit { get; set; } = string.Empty; // Đơn vị (Tấm/Mét)
        public decimal UnitPrice { get; set; } // Giá nhập
    }
}
