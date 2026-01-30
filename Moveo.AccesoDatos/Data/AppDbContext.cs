using Microsoft.EntityFrameworkCore;

using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>().ToTable("usuario");
        modelBuilder.Entity<Usuario>().Property(u => u.Id).HasColumnName("id");
        modelBuilder.Entity<Usuario>().Property(u => u.Nombre).HasColumnName("nombre");
        modelBuilder.Entity<Usuario>().Property(u => u.Email).HasColumnName("email");
        modelBuilder.Entity<Usuario>().Property(u => u.PasswordHash).HasColumnName("passwordhash");
        modelBuilder.Entity<Usuario>().Property(u => u.Rol).HasColumnName("rol");
        modelBuilder.Entity<Usuario>().Property(u => u.DebeCambiarPassword).HasColumnName("debecambiarpassword");
        modelBuilder.Entity<Usuario>().Property(u => u.FechaRegistro).HasColumnName("fecharegistro");
        modelBuilder.Entity<Usuario>().Property(u => u.CreatedAt).HasColumnName("created_at");
        modelBuilder.Entity<Usuario>().Property(u => u.UpdatedAt).HasColumnName("updated_at");

        modelBuilder.Entity<RefreshToken>().ToTable("refresh_tokens");
        modelBuilder.Entity<RefreshToken>().Property(t => t.Id).HasColumnName("id");
        modelBuilder.Entity<RefreshToken>().Property(t => t.Token).HasColumnName("token_hash");
        modelBuilder.Entity<RefreshToken>().Property(t => t.Expires).HasColumnName("expires_at");
        modelBuilder.Entity<RefreshToken>().Property(t => t.Created).HasColumnName("created_at");
        modelBuilder.Entity<RefreshToken>().Property(t => t.Revoked).HasColumnName("revoked");
        modelBuilder.Entity<RefreshToken>().Property(t => t.UserId).HasColumnName("usuario_id");
        modelBuilder.Entity<RefreshToken>().Property(t => t.FamilyId).HasColumnName("family_id");

        modelBuilder.Entity<RefreshToken>()
            .HasOne(t => t.Usuario)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(t => t.UserId);
    }
}
