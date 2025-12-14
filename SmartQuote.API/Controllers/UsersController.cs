using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartQuote.API.Data;
using SmartQuote.API.Entities;

namespace SmartQuote.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Chỉ Admin mới được gọi API này
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // 1. Lấy danh sách nhân viên
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            // Trả về list nhưng KHÔNG trả về PasswordHash (Bảo mật)
            return await _context.Users
                .Select(u => new User
                {
                    Id = u.Id,
                    Username = u.Username,
                    Role = u.Role,
                    PasswordHash = "" // Giấu pass
                })
                .ToListAsync();
        }

        // 2. Tạo nhân viên mới (Register nội bộ)
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(UserDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest("Tên tài khoản đã tồn tại!");
            }

            // Mã hóa mật khẩu
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                Role = request.Role // Role được chọn từ Frontend (Admin hoặc Staff)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Tạo tài khoản thành công" });
        }

        // 3. Xóa nhân viên (Nghỉ việc)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa nhân viên" });
        }
    }
}