using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moveo.Modelos.DTOs;
using Moveo.Negocio.Servicios;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Moveo.API.Controllers;

/// <summary>
/// Controlador para la gestión y planificación de rutas de transporte.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RutaController : ControllerBase
{
    private readonly IRutaService _rutaService;

    public RutaController(IRutaService rutaService)
    {
        _rutaService = rutaService;
    }

    /// <summary>
    /// Obtiene la lista de rutas con soporte para filtrado avanzado.
    /// </summary>
    /// <param name="estado">Filtrar por estado (Planificada, En Progreso, Completada, Cancelada).</param>
    /// <param name="conductorId">Filtrar por el ID del conductor asignado.</param>
    /// <param name="vehiculoId">Filtrar por el ID del vehículo asignado.</param>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RutaDto>>> ObtenerTodas(
        [FromQuery] string? estado = null,
        [FromQuery] int? conductorId = null,
        [FromQuery] int? vehiculoId = null)
    {
        var rutas = await _rutaService.ObtenerTodasAsync(estado, conductorId, vehiculoId);
        return Ok(rutas);
    }

    /// <summary>
    /// Obtiene las rutas asignadas al conductor autenticado.
    /// </summary>
    [HttpGet("me")]
    public async Task<ActionResult<IEnumerable<RutaDto>>> ObtenerMisRutas()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var rutas = await _rutaService.ObtenerMisRutasAsync(userId);
        return Ok(rutas);
    }

    /// <summary>
    /// Obtiene los detalles de una ruta específica por su ID, incluyendo el listado de entregas.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<RutaDetalleDto>> ObtenerPorId(int id)
    {
        var ruta = await _rutaService.ObtenerPorIdAsync(id);
        if (ruta == null) return NotFound();
        return Ok(ruta);
    }

    /// <summary>
    /// Obtiene un resumen estadístico de las rutas por su estado actual (Solo Administrador).
    /// </summary>
    [HttpGet("estadisticas")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<RutaEstadisticasDto>> ObtenerEstadisticas()
    {
        var stats = await _rutaService.ObtenerEstadisticasAsync();
        return Ok(stats);
    }

    /// <summary>
    /// Crea una nueva ruta en el sistema (Solo Administrador).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<RutaDto>> Crear(CrearRutaDto rutaDto)
    {
        var nuevaRuta = await _rutaService.CrearAsync(rutaDto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaRuta.Id }, nuevaRuta);
    }

    /// <summary>
    /// Actualiza la información completa de una ruta (Solo Administrador).
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult> Actualizar(int id, ActualizarRutaDto rutaDto)
    {
        var actualizado = await _rutaService.ActualizarAsync(id, rutaDto);
        if (!actualizado) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Actualiza únicamente el estado de una ruta (p.ej. para comenzar o finalizar el viaje).
    /// </summary>
    [HttpPatch("{id}/estado")]
    public async Task<ActionResult> ActualizarEstado(int id, ActualizarEstadoRutaDto dto)
    {
        var actualizado = await _rutaService.ActualizarEstadoAsync(id, dto.NuevoEstado);
        if (!actualizado) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Elimina una ruta del sistema (Solo Administrador).
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult> Eliminar(int id)
    {
        var eliminado = await _rutaService.EliminarAsync(id);
        if (!eliminado) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Optimiza el orden de las entregas de una ruta utilizando IA (Google Gemini), el clima actual y distancias.
    /// </summary>
    /// <param name="id">El ID de la ruta a optimizar.</param>
    [HttpPost("{id}/optimizar-ia")]
    public async Task<ActionResult<OptimizacionIaResponseDto>> OptimizarRutaConIa(int id)
    {
        try
        {
            var optimizacion = await _rutaService.OptimizarRutaAsync(id);
            return Ok(new
            {
                RutaId = id,
                Exito = true,
                Optimizacion = optimizacion
            });
        }
        catch (System.Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
