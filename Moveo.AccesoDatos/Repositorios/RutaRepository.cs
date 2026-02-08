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

    public async Task<IEnumerable<Ruta>> ObtenerTodasAsync()
    {
        return await _context.Rutas
            .Include(r => r.Conductor)
            .Include(r => r.Vehiculo)
            .ToListAsync();
    }

    public async Task<Ruta?> ObtenerPorIdAsync(int id)
    {
        return await _context.Rutas
            .Include(r => r.Conductor)
            .Include(r => r.Vehiculo)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Ruta>> ObtenerPorConductorIdAsync(int conductorId)
    {
        return await _context.Rutas
            .Include(r => r.Conductor)
            .Include(r => r.Vehiculo)
            .Where(r => r.ConductorId == conductorId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ruta>> ObtenerPorVehiculoIdAsync(int vehiculoId)
    {
        return await _context.Rutas
            .Include(r => r.Conductor)
            .Include(r => r.Vehiculo)
            .Where(r => r.VehiculoId == vehiculoId)
            .ToListAsync();
    }

    public async Task AgregarAsync(Ruta ruta)
    {
        await _context.Rutas.AddAsync(ruta);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Ruta ruta)
    {
        _context.Rutas.Update(ruta);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Ruta ruta)
    {
        _context.Rutas.Remove(ruta);
        await _context.SaveChangesAsync();
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }
}
