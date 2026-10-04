using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeVault.Api.Data;
using SafeVault.Api.Models;
using SafeVault.Api.Services;
using System.Security.Cryptography;
using System.Text;

namespace SafeVault.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly SafeVaultDbContext _db;
        private readonly AuthService _auth;

        public AuthController(SafeVaultDbContext db, AuthService auth)
        {
            _db = db;
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserAccount user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _db.Users.AnyAsync(u => u.Username == user.Username))
                return BadRequest("Username already exists.");

            user.PasswordHash = HashPassword(user.PasswordHash);
            user.Role = string.IsNullOrEmpty(user.Role) ? "User" : user.Role;

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Register), new { id = user.Id }, new
            {
                user.Id,
                user.Username,
                user.Email,
                user.Role
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var hash = HashPassword(request.Password);
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username && u.PasswordHash == hash);

            if (user == null)
                return Unauthorized("Invalid credentials.");

            var token = _auth.GenerateToken(user);
            return Ok(new { token });
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }

    public class LoginRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
