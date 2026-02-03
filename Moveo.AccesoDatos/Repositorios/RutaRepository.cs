using Microsoft.EntityFrameworkCore;
using Moveo.AccesoDatos.Data;
using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public class RutaRepository : IRutaRepository
{
    private readonly AppDbContext _context;

    public RutaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Ruta>> ObtenerTodosAsync()
    {
        return await _context.Rutas
            .Include(r => r.Conductor)
            .Include(r => r.Vehiculo)
            .Include(r => r.Entregas)
            .ToListAsync();
    }

    public async Task<Ruta?> ObtenerPorIdAsync(int id)
    {
        return await _context.Rutas
            .Include(r => r.Conductor)
            .Include(r => r.Vehiculo)
            .Include(r => r.Entregas)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task AgregarAsync(Ruta ruta)
    {
        await _context.Rutas.AddAsync(ruta);
        await _context.SaveChangesAsync();
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }
}
