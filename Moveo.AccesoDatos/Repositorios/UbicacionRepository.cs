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
}
