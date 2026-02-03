namespace Moveo.Modelos.DTOs;

public class EstadisticaUsuarioDto
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int PuntosAcumulados { get; set; }
    public decimal KilometrosAhorrados { get; set; }
    public int EntregasExitosas { get; set; }
}

public class ActualizarEstadisticaUsuarioDto
{
    public int PuntosAcumulados { get; set; }
    public decimal KilometrosAhorrados { get; set; }
    public int EntregasExitosas { get; set; }
}
