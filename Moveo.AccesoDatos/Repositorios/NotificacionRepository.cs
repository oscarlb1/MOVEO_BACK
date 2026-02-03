using Microsoft.EntityFrameworkCore;
using Moveo.AccesoDatos.Data;
using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public class NotificacionRepository : INotificacionRepository
{
    private readonly AppDbContext _context;

    public NotificacionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Notificacion>> ObtenerPorUsuarioIdAsync(int usuarioId)
    {
        return await _context.Notificaciones
            .Where(n => n.UsuarioId == usuarioId)
            .OrderByDescending(n => n.Fecha)
            .ToListAsync();
    }

    public async Task<Notificacion?> ObtenerPorIdAsync(int id)
    {
        return await _context.Notificaciones.FindAsync(id);
    }

    public async Task AgregarAsync(Notificacion notificacion)
    {
        await _context.Notificaciones.AddAsync(notificacion);
        await _context.SaveChangesAsync();
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }
}
