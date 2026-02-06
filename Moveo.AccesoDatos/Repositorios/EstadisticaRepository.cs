using Microsoft.EntityFrameworkCore;
using Moveo.AccesoDatos.Data;
using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public class EstadisticaRepository : IEstadisticaRepository
{
    private readonly AppDbContext _context;

    public EstadisticaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EstadisticaUsuario?> ObtenerPorUsuarioIdAsync(int usuarioId)
    {
        return await _context.EstadisticasUsuarios
            .Include(e => e.Usuario)
            .FirstOrDefaultAsync(e => e.UsuarioId == usuarioId);
    }

    public async Task<IEnumerable<EstadisticaUsuario>> ObtenerMejoresUsuariosAsync(int count, string? filtro = null, string? sortBy = "entregas")
    {
        var query = _context.EstadisticasUsuarios
            .Include(e => e.Usuario)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filtro))
        {
            query = query.Where(e => e.Usuario != null && e.Usuario.Nombre.Contains(filtro));
        }

        if (sortBy?.ToLower() == "puntos")
        {
            query = query.OrderByDescending(e => e.PuntosAcumulados);
        }
        else
        {
            query = query.OrderByDescending(e => e.EntregasExitosas);
        }

        return await query
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<EstadisticaUsuario>> ObtenerTodasAsync()
    {
        return await _context.EstadisticasUsuarios
            .Include(e => e.Usuario)
            .ToListAsync();
    }

    public async Task ActualizarAsync(EstadisticaUsuario estadistica)
    {
        _context.EstadisticasUsuarios.Update(estadistica);
        await _context.SaveChangesAsync();
    }

    public async Task AgregarAsync(EstadisticaUsuario estadistica)
    {
        await _context.EstadisticasUsuarios.AddAsync(estadistica);
        await _context.SaveChangesAsync();
    }

    public async Task IncrementarEstadisticasAsync(int usuarioId, int puntos, decimal km, int entregas)
    {
        var stats = await _context.EstadisticasUsuarios.FirstOrDefaultAsync(e => e.UsuarioId == usuarioId);
        if (stats == null)
        {
            stats = new EstadisticaUsuario
            {
                UsuarioId = usuarioId,
                PuntosAcumulados = puntos,
                KilometrosAhorrados = km,
                EntregasExitosas = entregas
            };
            await _context.EstadisticasUsuarios.AddAsync(stats);
        }
        else
        {
            stats.PuntosAcumulados += puntos;
            stats.KilometrosAhorrados += km;
            stats.EntregasExitosas += entregas;
        }
        await _context.SaveChangesAsync();
    }
}
