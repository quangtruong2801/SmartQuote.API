using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartQuote.API.DTOs.Upload;

namespace SmartQuote.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UploadController : ControllerBase
    {
        private readonly Cloudinary _cloudinary;

        private const long MAX_FILE_SIZE = 5 * 1024 * 1024;
        private static readonly string[] ALLOWED_EXTENSIONS =
            { ".jpg", ".jpeg", ".png", ".webp" };

        public UploadController(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        // POST: api/Upload/image
        [HttpPost("image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage([FromForm] ImageUploadDto dto)
        {
            var file = dto.File;

            if (file == null || file.Length == 0)
                return BadRequest("Vui lòng chọn file ảnh.");

            if (file.Length > MAX_FILE_SIZE)
                return BadRequest("Dung lượng ảnh tối đa 5MB.");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!ALLOWED_EXTENSIONS.Contains(ext))
                return BadRequest("Chỉ hỗ trợ JPG, JPEG, PNG, WEBP.");

            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "smartquote/product-templates",
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
                return StatusCode(500, result.Error.Message);

            return Ok(new
            {
                url = result.SecureUrl.ToString(),
                publicId = result.PublicId
            });
        }
    }
}
