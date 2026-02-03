namespace Moveo.Modelos.DTOs;

public class ClienteDto
{
    public int Id { get; set; }
    public string NombreEmpresa { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public double Latitud { get; set; }
    public double Longitud { get; set; }
    public string? Telefono { get; set; }
}

public class CrearClienteDto
{
    public string NombreEmpresa { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public double Latitud { get; set; }
    public double Longitud { get; set; }
    public string? Telefono { get; set; }
}

public class ActualizarClienteDto
{
    public string NombreEmpresa { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public double Latitud { get; set; }
    public double Longitud { get; set; }
    public string? Telefono { get; set; }
}
