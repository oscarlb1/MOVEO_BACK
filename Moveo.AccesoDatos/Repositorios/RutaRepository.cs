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

    public async Task<IEnumerable<Ruta>> ObtenerTodasAsync(string? estado = null, int? conductorId = null, int? vehiculoId = null)
    {
        var query = _context.Rutas
            .Include(r => r.Conductor)
            .Include(r => r.Vehiculo)
            .AsQueryable();

        if (!string.IsNullOrEmpty(estado))
        {
            query = query.Where(r => r.Estado == estado);
        }

        if (conductorId.HasValue)
        {
            query = query.Where(r => r.ConductorId == conductorId.Value);
        }

        if (vehiculoId.HasValue)
        {
            query = query.Where(r => r.VehiculoId == vehiculoId.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<int> ObtenerConteoPorEstadoAsync(string estado)
    {
        return await _context.Rutas.CountAsync(r => r.Estado == estado);
    }

    public async Task<Ruta?> ObtenerPorIdAsync(int id)
    {
        return await _context.Rutas
            .Include(r => r.Conductor)
            .Include(r => r.Vehiculo)
            .Include(r => r.Entregas)
                .ThenInclude(e => e.Cliente)
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
