namespace Moveo.Modelos.DTOs;

public class UsuarioDto
{
    public int Id { get; set; }
    public int VehiculoId { get; set; } = string.Empty;
    public string TipoMantenimiento { get; set; } = string.Empty;
    public int KilometrajeServicio { get; set; }
    public decimal Coste { get; set; };
}

public class CrearUsuarioDto
{
    public int Id { get; set; }
    public int VehiculoId { get; set; } = string.Empty;
    public string TipoMantenimiento { get; set; } = string.Empty;
    public int KilometrajeServicio { get; set; }
    public decimal Coste { get; set; };
}