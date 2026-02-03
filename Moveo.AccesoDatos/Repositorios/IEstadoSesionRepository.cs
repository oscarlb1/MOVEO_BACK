using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public interface IEstadoSesionRepository
{
    Task<EstadoSesion?> ObtenerPorUsuarioIdAsync(int usuarioId);
    Task AgregarAsync(EstadoSesion estadoSesion);
    Task GuardarCambiosAsync();
}
