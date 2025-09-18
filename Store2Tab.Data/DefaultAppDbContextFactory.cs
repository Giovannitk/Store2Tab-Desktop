using Microsoft.EntityFrameworkCore;

namespace Store2Tab.Data
{
    /// <summary>
    /// Minimal factory that relies on AppDbContext.OnConfiguring for the connection string.
    /// </summary>
    public class DefaultAppDbContextFactory : IDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext()
        {
            // Empty options so that OnConfiguring applies the fallback connection
            var options = new DbContextOptionsBuilder<AppDbContext>().Options;
            return new AppDbContext(options);
        }
    }
}


