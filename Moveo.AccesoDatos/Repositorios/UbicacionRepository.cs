using Microsoft.EntityFrameworkCore;
using Moveo.AccesoDatos.Data;
using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public class UbicacionRepository : IUbicacionRepository
{
    private readonly AppDbContext _context;

    public UbicacionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task RegistrarPuntoAsync(UbicacionHistorial punto)
    {
        await _context.HistorialUbicaciones.AddAsync(punto);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<UbicacionHistorial>> ObtenerPorRutaAsync(int rutaId)
    {
        return await _context.HistorialUbicaciones
            .Where(u => u.RutaId == rutaId)
            .OrderBy(u => u.FechaHora)
            .ToListAsync();
    }

    public async Task<UbicacionHistorial?> ObtenerPorIdAsync(int id)
    {
        return await _context.HistorialUbicaciones.FindAsync(id);
    }

    public async Task ActualizarPuntoAsync(UbicacionHistorial punto)
    {
        _context.HistorialUbicaciones.Update(punto);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarPuntoAsync(int id)
    {
        var punto = await ObtenerPorIdAsync(id);
        if (punto != null)
        {
            _context.HistorialUbicaciones.Remove(punto);
            await _context.SaveChangesAsync();
        }
    }

    public async Task EliminarPorRutaAsync(int rutaId)
    {
        var puntos = await _context.HistorialUbicaciones.Where(u => u.RutaId == rutaId).ToListAsync();
        _context.HistorialUbicaciones.RemoveRange(puntos);
        await _context.SaveChangesAsync();
    }
}
