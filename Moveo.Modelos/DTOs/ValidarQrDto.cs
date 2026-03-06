namespace Moveo.Modelos.DTOs;

public class ValidarQrDto
{
    public required string CodigoQr { get; set; }
    public string? FirmaBase64 { get; set; } 
}