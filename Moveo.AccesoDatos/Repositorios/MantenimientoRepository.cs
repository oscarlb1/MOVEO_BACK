using Microsoft.EntityFrameworkCore;
using Moveo.AccesoDatos.Data;
using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public class MantenimientoRepository : IMantenimientoRepository
{
    private readonly AppDbContext _context;

    public MantenimientoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Mantenimiento>> ObtenerTodosAsync()
    {
        return await _context.Mantenimientos.Include(m => m.Vehiculo).ToListAsync();
    }

    public async Task<Mantenimiento?> ObtenerPorIdAsync(int id)
    {
        return await _context.Mantenimientos.Include(m => m.Vehiculo).FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task AgregarAsync(Mantenimiento mantenimiento)
    {
        await _context.Mantenimientos.AddAsync(mantenimiento);
        await _context.SaveChangesAsync();
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }
    
}
