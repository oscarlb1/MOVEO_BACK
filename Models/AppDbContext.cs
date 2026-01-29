using Microsoft.EntityFrameworkCore;

namespace MoveoBack.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>().ToTable("usuario");
        modelBuilder.Entity<RefreshToken>().ToTable("refresh_tokens");

        // Map C# properties to snake_case columns if they differ
        modelBuilder.Entity<Usuario>().Property(u => u.Nombre).HasColumnName("nombre");
        modelBuilder.Entity<Usuario>().Property(u => u.PasswordHash).HasColumnName("passwordhash");
    }
}
