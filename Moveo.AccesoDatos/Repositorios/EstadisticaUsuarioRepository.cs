using Microsoft.EntityFrameworkCore;
using Moveo.AccesoDatos.Data;
using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public class EstadisticaUsuarioRepository : IEstadisticaUsuarioRepository
{
    private readonly AppDbContext _context;

    public EstadisticaUsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EstadisticaUsuario?> ObtenerPorUsuarioIdAsync(int usuarioId)
    {
        return await _context.EstadisticasUsuarios
            .Include(e => e.Usuario)
            .FirstOrDefaultAsync(e => e.UsuarioId == usuarioId);
    }

    public async Task AgregarAsync(EstadisticaUsuario estadistica)
    {
        await _context.EstadisticasUsuarios.AddAsync(estadistica);
        await _context.SaveChangesAsync();
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }
}
