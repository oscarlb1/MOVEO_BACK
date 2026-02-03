namespace Moveo.Modelos.DTOs;

public class MantenimientoDto
{
    public int Id { get; set; }
    public int VehiculoId { get; set; }
    public DateTime FechaServicio { get; set; }
    public string TipoMantenimiento { get; set; } = string.Empty;
    public int KilometrajeServicio { get; set; }
    public decimal Coste { get; set; }
}

public class CrearMantenimientoDto
{
    public int VehiculoId { get; set; }
    public DateTime FechaServicio { get; set; }
    public string TipoMantenimiento { get; set; } = string.Empty;
    public int KilometrajeServicio { get; set; }
    public decimal Coste { get; set; }
}

public class ActualizarMantenimientoDto
{
    public DateTime FechaServicio { get; set; }
    public string TipoMantenimiento { get; set; } = string.Empty;
    public int KilometrajeServicio { get; set; }
    public decimal Coste { get; set; }
}