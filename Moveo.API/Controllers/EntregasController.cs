using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Moveo.Modelos.DTOs;
using Moveo.Negocio.Servicios;

namespace Moveo.API.Controllers;

/// <summary>
/// Controlador para la gestión y seguimiento de entregas y rutas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EntregasController : ControllerBase
{
    private readonly IEntregaService _entregaService;

    public EntregasController(IEntregaService entregaService)
    {
        _entregaService = entregaService;
    }

    /// <summary>
    /// Obtiene todas las entregas filtradas por ruta, cliente, estado o fecha.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EntregaDto>>> ObtenerTodas(
        [FromQuery] int? rutaId,
        [FromQuery] int? clienteId,
        [FromQuery] string? estado,
        [FromQuery] DateTime? fecha)
    {
        var entregas = await _entregaService.ObtenerTodasAsync(rutaId, clienteId, estado, fecha);
        return Ok(entregas);
    }

    /// <summary>
    /// Obtiene los detalles de una entrega específica por su ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EntregaDto>> ObtenerPorId(int id)
    {
        var entrega = await _entregaService.ObtenerPorIdAsync(id);
        if (entrega == null) return NotFound();
        return Ok(entrega);
    }

    /// <summary>
    /// Obtiene todas las entregas asociadas a una ruta específica.
    /// </summary>
    [HttpGet("rutas/{rutaId}")]
    public async Task<ActionResult<IEnumerable<EntregaDto>>> ObtenerPorRuta(int rutaId)
    {
        var entregas = await _entregaService.ObtenerPorRutaAsync(rutaId);
        return Ok(entregas);
    }

    /// <summary>
    /// Registra una nueva entrega en el sistema. (Solo Administrador)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<EntregaDto>> Crear(CrearEntregaDto crearEntregaDto)
    {
        try
        {
            var entrega = await _entregaService.CrearAsync(crearEntregaDto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = entrega.Id }, entrega);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza los datos completos de una entrega. (Solo Administrador)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<EntregaDto>> Actualizar(int id, ActualizarEntregaDto dto)
    {
        try
        {
            var entrega = await _entregaService.ActualizarAsync(id, dto);
            if (entrega == null) return NotFound();
            return Ok(entrega);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza el estado de una entrega (p.ej. 'EN_CAMINO', 'ENTREGADO').
    /// </summary>
    [HttpPut("{id}/estado")]
    public async Task<ActionResult> ActualizarEstado(int id, ActualizarEstadoEntregaDto dto)
    {
        try
        {
            await _entregaService.ActualizarEstadoAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Elimina una entrega del sistema. (Solo Administrador)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult> Eliminar(int id)
    {
        await _entregaService.EliminarAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Obtiene un resumen estadístico de las entregas del día de hoy.
    /// </summary>
    [HttpGet("estadisticas/hoy")]
    public async Task<ActionResult<EntregaEstadisticasDto>> ObtenerEstadisticasHoy()
    {
        var estadisticas = await _entregaService.ObtenerEstadisticasDelDiaAsync();
        return Ok(estadisticas);
    }

    /// <summary>
    /// Valida el código QR escaneado por el repartidor para una entrega específica.
    /// </summary>
    [HttpPost("{id}/validar-qr")]
    public async Task<IActionResult> ValidarCodigoQR(int id, [FromBody] ValidarQrDto dto)
    {
        var esValido = await _entregaService.ValidarCodigoQRAsync(id, dto.CodigoQr);
        if (esValido)
        {
            return Ok(new { message = "Código QR verificado correctamente." });
        }

        return BadRequest(new { message = "El código QR es incorrecto o no pertenece a esta entrega." });
    }
}
