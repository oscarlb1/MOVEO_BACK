namespace Moveo.Modelos.DTOs;

public class EstadisticaUsuarioDto
{
    public int UsuarioId { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public int PuntosAcumulados { get; set; }
    public decimal KilometrosAhorrados { get; set; }
    public int EntregasTotales { get; set; }
}

public class RankingUsuarioDto
{
    public int Posicion { get; set; }
    public int UsuarioId { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public int Puntos { get; set; }
    public int EntregasTotales { get; set; }
    public string? ImagenUrl { get; set; }
}

public class EstadisticaGlobalDto
{
    public int TotalUsuarios { get; set; }
    public int TotalEntregasTotales { get; set; }
    public decimal TotalKilometrosAhorrados { get; set; }
    public int TotalPuntosAcumulados { get; set; }
}

public record EstadisticaHoyDto(
    int EntregasTotales,
    int EntregasCompletadas,
    decimal Eficiencia,
    string TiempoEnRuta
);
