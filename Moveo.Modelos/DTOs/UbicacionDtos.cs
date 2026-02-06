namespace Moveo.Modelos.DTOs;

public class UbicacionDto
{
    public int Id { get; set; }
    public int RutaId { get; set; }
    public double Latitud { get; set; }
    public double Longitud { get; set; }
    public DateTime FechaHora { get; set; }
}

public class RegistroUbicacionDto
{
    public int RutaId { get; set; }
    public double Latitud { get; set; }
    public double Longitud { get; set; }
}
