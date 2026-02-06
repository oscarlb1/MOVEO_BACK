using Microsoft.EntityFrameworkCore;
using Moveo.Modelos.Entidades;
using Moveo.AccesoDatos.Data;

namespace Moveo.AccesoDatos.Repositorios;

public class VehiculoRepository : IVehiculoRepository
{
    private readonly AppDbContext _context;

    public VehiculoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Vehiculo>> ObtenerTodosAsync()
    {
        return await _context.Vehiculos.Where(v => v.DeletedAt == null).ToListAsync();
    }

    public async Task<Vehiculo?> ObtenerPorIdAsync(int id)
    {
        return await _context.Vehiculos.FirstOrDefaultAsync(v => v.Id == id && v.DeletedAt == null);
    }

    public async Task ActualizarAsync(Vehiculo vehiculo)
    {
        vehiculo.UpdatedAt = DateTime.UtcNow;
        _context.Vehiculos.Update(vehiculo);
        await Task.CompletedTask;
    }

    public async Task AgregarAsync(Vehiculo vehiculo)
    {
        await _context.Vehiculos.AddAsync(vehiculo);
    }

    public async Task EliminarAsync(int id)
    {
        var vehiculo = await ObtenerPorIdAsync(id);
        if (vehiculo != null)
        {
            vehiculo.DeletedAt = DateTime.UtcNow;
            _context.Vehiculos.Update(vehiculo);
        }
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }
}
