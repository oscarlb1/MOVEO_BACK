using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public interface IEstadisticaUsuarioRepository
{
    Task<EstadisticaUsuario?> ObtenerPorUsuarioIdAsync(int usuarioId);
    Task AgregarAsync(EstadisticaUsuario estadistica);
    Task GuardarCambiosAsync();
}
