using Microsoft.EntityFrameworkCore;
using SafeVault.Api.Models;

namespace SafeVault.Api.Data
{
    public class SafeVaultDbContext : DbContext
    {
        public SafeVaultDbContext(DbContextOptions<SafeVaultDbContext> options)
            : base(options) { }

        public DbSet<UserAccount> Users { get; set; }
    }
}
