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
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Entrega> Entregas { get; set; }
    public DbSet<EstadisticaUsuario> EstadisticasUsuarios { get; set; }
    public DbSet<EstadoSesion> EstadosSesiones { get; set; }
    public DbSet<UbicacionHistorial> HistorialUbicaciones { get; set; }

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
        modelBuilder.Entity<Usuario>().Property(u => u.UltimaConexion).HasColumnName("ultimaconexion");
        modelBuilder.Entity<Usuario>().Property(u => u.ImagenUrl).HasColumnName("imagen_url");
        modelBuilder.Entity<Usuario>().Property(u => u.Telefono).HasColumnName("telefono");

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

        modelBuilder.Entity<Cliente>().ToTable("cliente");
        modelBuilder.Entity<Cliente>().Property(c => c.Id).HasColumnName("id");
        modelBuilder.Entity<Cliente>().Property(c => c.NombreEmpresa).HasColumnName("nombreempresa");
        modelBuilder.Entity<Cliente>().Property(c => c.Direccion).HasColumnName("direccion");
        modelBuilder.Entity<Cliente>().Property(c => c.Telefono).HasColumnName("telefono");
        modelBuilder.Entity<Cliente>().Property(c => c.Latitud).HasColumnName("latitud");
        modelBuilder.Entity<Cliente>().Property(c => c.Longitud).HasColumnName("longitud");
        modelBuilder.Entity<Cliente>().Property(c => c.CreatedAt).HasColumnName("created_at");
        modelBuilder.Entity<Cliente>().Property(c => c.UpdatedAt).HasColumnName("updated_at");
        modelBuilder.Entity<Cliente>().Property(c => c.DeletedAt).HasColumnName("deleted_at");

        modelBuilder.Entity<Entrega>().ToTable("entrega");
        modelBuilder.Entity<Entrega>().Property(e => e.Id).HasColumnName("id");
        modelBuilder.Entity<Entrega>().Property(e => e.RutaId).HasColumnName("rutaid");
        modelBuilder.Entity<Entrega>().Property(e => e.ClienteId).HasColumnName("clienteid");
        modelBuilder.Entity<Entrega>().Property(e => e.OrdenParada).HasColumnName("ordenparada");
        modelBuilder.Entity<Entrega>().Property(e => e.HoraEntregaReal).HasColumnName("horaentregareal");
        modelBuilder.Entity<Entrega>().Property(e => e.FotoUrl).HasColumnName("fotourl");
        modelBuilder.Entity<Entrega>().Property(e => e.FirmaDigitalUrl).HasColumnName("firmadigitalurl");
        modelBuilder.Entity<Entrega>().Property(e => e.Notas).HasColumnName("notas");
        modelBuilder.Entity<Entrega>().Property(e => e.Estado).HasColumnName("estado");
        modelBuilder.Entity<Entrega>().Property(e => e.CodigoQr).HasColumnName("codigoqr");
        modelBuilder.Entity<Entrega>().Property(e => e.CreatedAt).HasColumnName("created_at");
        modelBuilder.Entity<Entrega>().Property(e => e.UpdatedAt).HasColumnName("updated_at");

        modelBuilder.Entity<Entrega>()
            .HasOne(e => e.Cliente)
            .WithMany()
            .HasForeignKey(e => e.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EstadisticaUsuario>().ToTable("estadisticausuario");
        modelBuilder.Entity<EstadisticaUsuario>().Property(e => e.Id).HasColumnName("id");
        modelBuilder.Entity<EstadisticaUsuario>().Property(e => e.UsuarioId).HasColumnName("usuarioid");
        modelBuilder.Entity<EstadisticaUsuario>().Property(e => e.PuntosAcumulados).HasColumnName("puntosacumulados");
        modelBuilder.Entity<EstadisticaUsuario>().Property(e => e.KilometrosAhorrados).HasColumnName("kilometrosahorrados");
        modelBuilder.Entity<EstadisticaUsuario>().Property(e => e.EntregasExitosas).HasColumnName("entregasexitosas");

        modelBuilder.Entity<EstadisticaUsuario>()
            .HasOne(e => e.Usuario)
            .WithOne()
            .HasForeignKey<EstadisticaUsuario>(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EstadoSesion>().ToTable("estado_sesion");
        modelBuilder.Entity<EstadoSesion>().Property(e => e.Id).HasColumnName("id");
        modelBuilder.Entity<EstadoSesion>().Property(e => e.UsuarioId).HasColumnName("usuario_id");
        modelBuilder.Entity<EstadoSesion>().Property(e => e.EstaEnLinea).HasColumnName("esta_en_linea");
        modelBuilder.Entity<EstadoSesion>().Property(e => e.UltimaConexion).HasColumnName("ultima_conexion");
        modelBuilder.Entity<EstadoSesion>().Property(e => e.Dispositivo).HasColumnName("dispositivo");

        modelBuilder.Entity<UbicacionHistorial>().ToTable("historialubicacion");
        modelBuilder.Entity<UbicacionHistorial>().Property(u => u.Id).HasColumnName("id");
        modelBuilder.Entity<UbicacionHistorial>().Property(u => u.RutaId).HasColumnName("rutaid");
        modelBuilder.Entity<UbicacionHistorial>().Property(u => u.Latitud).HasColumnName("latitud");
        modelBuilder.Entity<UbicacionHistorial>().Property(u => u.Longitud).HasColumnName("longitud");
        modelBuilder.Entity<UbicacionHistorial>().Property(u => u.FechaHora).HasColumnName("fechahora");
    }
}
