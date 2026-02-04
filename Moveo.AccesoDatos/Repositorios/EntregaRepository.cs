using Microsoft.EntityFrameworkCore;
using Moveo.AccesoDatos.Data;
using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public class EntregaRepository : IEntregaRepository
{
    private readonly AppDbContext _context;

    public EntregaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Entrega>> ObtenerTodasAsync()
    {
        return await _context.Entregas
            .Include(e => e.Cliente)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<Entrega?> ObtenerPorIdAsync(int id)
    {
        return await _context.Entregas
            .Include(e => e.Cliente)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Entrega>> ObtenerPorRutaAsync(int rutaId)
    {
        return await _context.Entregas
            .Include(e => e.Cliente)
            .Where(e => e.RutaId == rutaId)
            .OrderBy(e => e.OrdenParada)
            .ToListAsync();
    }

    public async Task<IEnumerable<Entrega>> ObtenerPorFiltrosAsync(int? rutaId, int? clienteId, string? estado, DateTime? fecha)
    {
        var query = _context.Entregas.Include(e => e.Cliente).AsQueryable();

        if (rutaId.HasValue)
            query = query.Where(e => e.RutaId == rutaId.Value);

        if (clienteId.HasValue)
            query = query.Where(e => e.ClienteId == clienteId.Value);

        if (!string.IsNullOrEmpty(estado))
            query = query.Where(e => e.Estado.ToLower() == estado.ToLower());

        if (fecha.HasValue)
        {
            // Comparar solo la fecha, ignorando la hora
            var fechaInicio = fecha.Value.Date;
            var fechaFin = fechaInicio.AddDays(1);
            query = query.Where(e => e.CreatedAt >= fechaInicio && e.CreatedAt < fechaFin);
        }

        return await query.OrderByDescending(e => e.CreatedAt).ToListAsync();
    }

    public async Task AgregarAsync(Entrega entrega)
    {
        await _context.Entregas.AddAsync(entrega);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Entrega entrega)
    {
        _context.Entregas.Update(entrega);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var entrega = await _context.Entregas.FindAsync(id);
        if (entrega != null)
        {
            _context.Entregas.Remove(entrega);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Entrega>> ObtenerDelDiaAsync(DateTime fecha)
    {
        var fechaInicio = fecha.Date;
        var fechaFin = fechaInicio.AddDays(1);
        return await _context.Entregas
            .Where(e => e.CreatedAt >= fechaInicio && e.CreatedAt < fechaFin)
            .ToListAsync();
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }
}
