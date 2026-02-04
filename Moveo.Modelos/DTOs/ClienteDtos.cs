namespace Moveo.Modelos.DTOs;

public record ClienteDto(
    int Id,
    string NombreEmpresa,
    string Direccion,
    string Telefono,
    double? Latitud,
    double? Longitud,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CrearClienteDto(
    string NombreEmpresa,
    string Direccion,
    string Telefono,
    double? Latitud,
    double? Longitud
);
