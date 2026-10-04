using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeVault.Api.Data;
using SafeVault.Api.Models;

namespace SafeVault.Api.Controllers
{
    [ApiController]
    [Route("api/vault")]
    [Authorize] // requires valid JWT
    public class VaultController : ControllerBase
    {
        private readonly SafeVaultDbContext _db;

        public VaultController(SafeVaultDbContext db)
        {
            _db = db;
        }

        // Example: Admin-only endpoint
        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _db.Users
                .Select(u => new { u.Id, u.Username, u.Email, u.Role })
                .ToListAsync();

            return Ok(users);
        }

        // Example: User-specific data (no raw SQL, EF prevents injection)
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null) return NotFound();

            return Ok(new { user.Id, user.Username, user.Email, user.Role });
        }
    }
}
