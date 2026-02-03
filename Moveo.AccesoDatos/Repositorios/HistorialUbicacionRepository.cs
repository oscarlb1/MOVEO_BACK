using Microsoft.EntityFrameworkCore;
using Moveo.AccesoDatos.Data;
using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public class HistorialUbicacionRepository : IHistorialUbicacionRepository
{
    private readonly AppDbContext _context;

    public HistorialUbicacionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<HistorialUbicacion>> ObtenerTodosAsync()
    {
        return await _context.HistorialUbicaciones.ToListAsync();
    }

    public async Task<IEnumerable<HistorialUbicacion>> ObtenerPorRutaIdAsync(int rutaId)
    {
        return await _context.HistorialUbicaciones
            .Where(h => h.RutaId == rutaId)
            .OrderBy(h => h.FechaHora)
            .ToListAsync();
    }

    public async Task AgregarAsync(HistorialUbicacion historial)
    {
        await _context.HistorialUbicaciones.AddAsync(historial);
        await _context.SaveChangesAsync();
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }
}
