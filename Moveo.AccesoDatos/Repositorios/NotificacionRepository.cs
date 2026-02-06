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

    public async Task<IEnumerable<Notificacion>> ObtenerTodasAsync()
    {
        return await _context.Notificaciones
            .Include(n => n.Usuario)
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

    public async Task AgregarVariasAsync(IEnumerable<Notificacion> notificaciones)
    {
        await _context.Notificaciones.AddRangeAsync(notificaciones);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Notificacion notificacion)
    {
        _context.Notificaciones.Update(notificacion);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var notif = await ObtenerPorIdAsync(id);
        if (notif != null)
        {
            _context.Notificaciones.Remove(notif);
            await _context.SaveChangesAsync();
        }
    }

    public async Task MarcarTodasComoLeidasAsync(int usuarioId)
    {
        var noLeidas = await _context.Notificaciones
            .Where(n => n.UsuarioId == usuarioId && !n.Leido)
            .ToListAsync();

        foreach (var n in noLeidas)
        {
            n.Leido = true;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<int> ObtenerConteoNoLeidasAsync(int usuarioId)
    {
        return await _context.Notificaciones
            .CountAsync(n => n.UsuarioId == usuarioId && !n.Leido);
    }
}
