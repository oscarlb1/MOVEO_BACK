using Microsoft.EntityFrameworkCore;
using Moveo.Modelos.Entidades;
using Moveo.AccesoDatos.Data;

namespace Moveo.AccesoDatos.Repositorios;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Usuario>> ObtenerTodosAsync()
    {
        return await _context.Usuarios.ToListAsync();
    }

    public async Task<Usuario?> ObtenerPorIdAsync(int id)
    {
        return await _context.Usuarios.FindAsync(id);
    }

    public async Task<Usuario?> ObtenerPorEmailAsync(string email)
    {
        return await _context.Usuarios.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AgregarAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task<Usuario?> ObtenerPorTokenDeRefrescoAsync(string token)
    {
        return await _context.Usuarios
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
    }

    public async Task EliminarAsync(int id)
    {
        var user = await _context.Usuarios.FindAsync(id);
        if (user != null)
        {
            _context.Usuarios.Remove(user);
        }
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }
}
