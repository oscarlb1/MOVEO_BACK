namespace Moveo.Modelos.DTOs;

public record EntregaDto(
    int Id,
    int RutaId,
    int ClienteId,
    ClienteDto? Cliente,
    int OrdenParada,
    string Estado,
    DateTime? HoraEntregaReal,
    string? FotoUrl,
    string? FirmaDigitalUrl,
    string? Notas,
    string? CodigoQr,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CrearEntregaDto(
    int RutaId,
    int ClienteId,
    int OrdenParada,
    string? Notas,
    string? CodigoQr
);

public record ActualizarEntregaDto(
    int ClienteId,
    int RutaId,
    int OrdenParada,
    string? Notas,
    string? CodigoQr
);

public record ActualizarEstadoEntregaDto(
    string Estado,
    string? FotoUrl,
    string? FirmaDigitalUrl,
    string? Notas
);

public record EntregaEstadisticasDto(
    int TotalHoy,
    int Pendientes,
    int Completadas,
    int Fallidas
);
