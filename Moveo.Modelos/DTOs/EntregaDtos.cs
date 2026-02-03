namespace Moveo.Modelos.DTOs;

public class EntregaDto
{
    public int Id { get; set; }
    public int RutaId { get; set; }
    public int ClienteId { get; set; }
    public int OrdenParada { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? CodigoQR { get; set; }
    public string? FirmaDigitalUrl { get; set; }
    public string? FotoUrl { get; set; }
    public string? Notas { get; set; }
    public DateTime? HoraEntregaReal { get; set; }
}

public class CrearEntregaDto
{
    public int RutaId { get; set; }
    public int ClienteId { get; set; }
    public int OrdenParada { get; set; }
    public string? Notas { get; set; }
}

public class ActualizarEntregaDto
{
    public int OrdenParada { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? CodigoQR { get; set; }
    public string? FirmaDigitalUrl { get; set; }
    public string? FotoUrl { get; set; }
    public string? Notas { get; set; }
    public DateTime? HoraEntregaReal { get; set; }
}
