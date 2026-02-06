using Microsoft.EntityFrameworkCore;
using Moveo.AccesoDatos.Data;
using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public class EstadoSesionRepository : IEstadoSesionRepository
{
    private readonly AppDbContext _context;

    public EstadoSesionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EstadoSesion?> ObtenerPorIdAsync(int id)
    {
        return await _context.EstadosSesiones
            .Include(e => e.Usuario)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<EstadoSesion?> ObtenerActivaPorUsuarioIdAsync(int usuarioId)
    {
        return await _context.EstadosSesiones
            .Include(e => e.Usuario)
            .FirstOrDefaultAsync(e => e.UsuarioId == usuarioId);
    }

    public async Task<IEnumerable<EstadoSesion>> ObtenerActivasAsync()
    {
        return await _context.EstadosSesiones
            .Include(e => e.Usuario)
            .Where(e => e.EstaEnLinea && e.UltimaConexion > DateTime.UtcNow.AddMinutes(-10))
            .ToListAsync();
    }

    public async Task<IEnumerable<EstadoSesion>> ObtenerPorUsuarioIdAsync(int usuarioId)
    {
        return await _context.EstadosSesiones
            .Include(e => e.Usuario)
            .Where(e => e.UsuarioId == usuarioId)
            .ToListAsync();
    }

    public async Task AgregarAsync(EstadoSesion sesion)
    {
        await _context.EstadosSesiones.AddAsync(sesion);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(EstadoSesion sesion)
    {
        _context.EstadosSesiones.Update(sesion);
        await _context.SaveChangesAsync();
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }
}
