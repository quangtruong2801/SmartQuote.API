using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartQuote.API.Entities
{
    public class ProductTemplate
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty; // VD: Tủ bếp dưới

        public string? ImageUrl { get; set; } // Link ảnh minh họa

        public string? ImagePublicId { get; set; }

        // Kích thước mặc định (để hiện lên form cho khách dễ hình dung)
        public double DefaultWidth { get; set; }
        public double DefaultHeight { get; set; }
        public double DefaultDepth { get; set; }

        // Công thức tính giá (Lưu chuỗi để sau này xử lý)
        // VD: "W*H*Material" hoặc "Fixed"
        public string PricingFormula { get; set; } = "W*H*Material";

        // Giá gia công (Labor cost) cộng thêm vào mỗi m2 hoặc mỗi cái
        public decimal BaseLaborCost { get; set; }

        // Khóa ngoại: Mặc định dùng loại vật tư nào? (VD: Gỗ MDF)
        public int DefaultMaterialId { get; set; }

        [ForeignKey("DefaultMaterialId")]
        public Material? DefaultMaterial { get; set; }
    }
}