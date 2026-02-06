using Microsoft.EntityFrameworkCore;

using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Vehiculo> Vehiculos { get; set; }
    public DbSet<Mantenimiento> Mantenimientos { get; set; }
    public DbSet<Ruta> Rutas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>().ToTable("usuario");
        // ... (existing mappings)
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

        modelBuilder.Entity<Vehiculo>().ToTable("vehiculo");
        modelBuilder.Entity<Vehiculo>().Property(v => v.Id).HasColumnName("id");
        modelBuilder.Entity<Vehiculo>().Property(v => v.Matricula).HasColumnName("matricula");
        modelBuilder.Entity<Vehiculo>().Property(v => v.MarcaModelo).HasColumnName("marcamodelo");
        modelBuilder.Entity<Vehiculo>().Property(v => v.Estado).HasColumnName("estado");
        modelBuilder.Entity<Vehiculo>().Property(v => v.CapacidadCarga).HasColumnName("capacidadcarga");
        modelBuilder.Entity<Vehiculo>().Property(v => v.ConsumoMedio).HasColumnName("consumomedio");
        modelBuilder.Entity<Vehiculo>().Property(v => v.KilometrajeActual).HasColumnName("kilometrajeactual");
        modelBuilder.Entity<Vehiculo>().Property(v => v.FechaUltimaRevision).HasColumnName("fechaultimarevision");
        modelBuilder.Entity<Vehiculo>().Property(v => v.CreatedAt).HasColumnName("created_at");
        modelBuilder.Entity<Vehiculo>().Property(v => v.UpdatedAt).HasColumnName("updated_at");
        modelBuilder.Entity<Vehiculo>().Property(v => v.DeletedAt).HasColumnName("deleted_at");

        modelBuilder.Entity<Mantenimiento>().ToTable("mantenimiento");
        modelBuilder.Entity<Mantenimiento>().Property(m => m.Id).HasColumnName("id");
        modelBuilder.Entity<Mantenimiento>().Property(m => m.VehiculoId).HasColumnName("vehiculoid");
        modelBuilder.Entity<Mantenimiento>().Property(m => m.FechaServicio).HasColumnName("fechaservicio");
        modelBuilder.Entity<Mantenimiento>().Property(m => m.TipoMantenimiento).HasColumnName("tipomantenimiento");
        modelBuilder.Entity<Mantenimiento>().Property(m => m.KilometrajeServicio).HasColumnName("kilometrajeservicio");
        modelBuilder.Entity<Mantenimiento>().Property(m => m.Coste).HasColumnName("coste");

        modelBuilder.Entity<Mantenimiento>()
            .HasOne(m => m.Vehiculo)
            .WithMany(v => v.Mantenimientos)
            .HasForeignKey(m => m.VehiculoId)
            .HasConstraintName("fk_vehiculo_mantenimiento");

        modelBuilder.Entity<Ruta>().ToTable("ruta");
        modelBuilder.Entity<Ruta>().Property(r => r.Id).HasColumnName("id");
        modelBuilder.Entity<Ruta>().Property(r => r.Fecha).HasColumnName("fecha");
        modelBuilder.Entity<Ruta>().Property(r => r.ConductorId).HasColumnName("conductorid");
        modelBuilder.Entity<Ruta>().Property(r => r.VehiculoId).HasColumnName("vehiculoid");
        modelBuilder.Entity<Ruta>().Property(r => r.Estado).HasColumnName("estado");
        modelBuilder.Entity<Ruta>().Property(r => r.DistanciaTotalEstimada).HasColumnName("distanciatotalestimada");

        modelBuilder.Entity<Ruta>()
            .HasOne(r => r.Conductor)
            .WithMany()
            .HasForeignKey(r => r.ConductorId)
            .HasConstraintName("fk_conductor_ruta");

        modelBuilder.Entity<Ruta>()
            .HasOne(r => r.Vehiculo)
            .WithMany()
            .HasForeignKey(r => r.VehiculoId)
            .HasConstraintName("fk_vehiculo_ruta");
    }
}
