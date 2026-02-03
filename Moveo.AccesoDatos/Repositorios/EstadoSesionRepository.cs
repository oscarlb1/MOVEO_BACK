using Microsoft.EntityFrameworkCore;
using Moveo.AccesoDatos.Data;
using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public class EstadoSesionRepository : IEstadoSesionRepository
{
    private readonly AppDbContext _context;

    public EstadoSesionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EstadoSesion?> ObtenerPorUsuarioIdAsync(int usuarioId)
    {
        return await _context.EstadoSesiones
            .Include(e => e.Usuario)
            .FirstOrDefaultAsync(e => e.UsuarioId == usuarioId);
    }

    public async Task AgregarAsync(EstadoSesion estadoSesion)
    {
        await _context.EstadoSesiones.AddAsync(estadoSesion);
        await _context.SaveChangesAsync();
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }
}
