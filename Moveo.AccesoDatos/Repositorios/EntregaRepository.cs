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

    public async Task<IEnumerable<Entrega>> ObtenerTodosAsync()
    {
        return await _context.Entregas
            .Include(e => e.Ruta)
            .Include(e => e.Cliente)
            .ToListAsync();
    }

    public async Task<Entrega?> ObtenerPorIdAsync(int id)
    {
        return await _context.Entregas
            .Include(e => e.Ruta)
            .Include(e => e.Cliente)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task AgregarAsync(Entrega entrega)
    {
        await _context.Entregas.AddAsync(entrega);
        await _context.SaveChangesAsync();
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }
}
