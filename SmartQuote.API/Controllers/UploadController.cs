using Microsoft.AspNetCore.Mvc;

namespace SmartQuote.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public UploadController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Vui lòng chọn file ảnh!");

            // 1. Tạo tên file độc nhất (tránh bị trùng tên)
            // Ví dụ: tu-bep.jpg -> tu-bep-GUID123.jpg
            var fileName = Path.GetFileNameWithoutExtension(file.FileName)
                           + "_"
                           + Guid.NewGuid().ToString()
                           + Path.GetExtension(file.FileName);

            // 2. Xác định đường dẫn lưu (vào thư mục wwwroot/images)
            var uploadFolder = Path.Combine(_environment.WebRootPath, "images");

            // Nếu thư mục chưa có thì tạo mới
            if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);

            var filePath = Path.Combine(uploadFolder, fileName);

            // 3. Lưu file vào ổ cứng
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 4. Trả về đường dẫn URL để Frontend hiển thị
            // Ví dụ: https://localhost:7207/images/ten-file.jpg
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var fileUrl = $"{baseUrl}/images/{fileName}";

            return Ok(new { url = fileUrl });
        }
    }
}