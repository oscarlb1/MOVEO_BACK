using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moveo.Modelos.DTOs;
using Moveo.Negocio.Servicios;

namespace Moveo.API.Controllers;

/// <summary>
/// Controlador para el registro y consulta de la ubicación geográfica de los vehículos y rutas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UbicacionController : ControllerBase
{
    private readonly IUbicacionService _ubicacionService;

    public UbicacionController(IUbicacionService ubicacionService)
    {
        _ubicacionService = ubicacionService;
    }

    /// <summary>
    /// Registra un nuevo punto de ubicación geográfica.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Registrar(RegistroUbicacionDto registroDto)
    {
        await _ubicacionService.RegistrarUbicacionAsync(registroDto);
        return Ok();
    }

    /// <summary>
    /// Obtiene el historial completo de ubicaciones registradas para una ruta específica.
    /// </summary>
    [HttpGet("ruta/{rutaId}")]
    public async Task<ActionResult<IEnumerable<UbicacionDto>>> ObtenerPorRuta(int rutaId)
    {
        var historial = await _ubicacionService.ObtenerHistorialPorRutaAsync(rutaId);
        return Ok(historial);
    }

    /// <summary>
    /// Obtiene la última ubicación conocida de una ruta específica.
    /// </summary>
    [HttpGet("ultimo/ruta/{rutaId}")]
    public async Task<ActionResult<UbicacionDto>> ObtenerUltimoPunto(int rutaId)
    {
        var punto = await _ubicacionService.ObtenerUltimaUbicacionAsync(rutaId);
        if (punto == null) return NotFound();
        return Ok(punto);
    }

    /// <summary>
    /// Actualiza un registro de ubicación específica. (Solo Administrador)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Actualizar(int id, RegistroUbicacionDto registroDto)
    {
        var success = await _ubicacionService.ActualizarUbicacionAsync(id, registroDto);
        if (!success) return NotFound();
        return Ok();
    }

    /// <summary>
    /// Elimina un punto de ubicación específico. (Solo Administrador)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var success = await _ubicacionService.EliminarUbicacionAsync(id);
        if (!success) return NotFound();
        return Ok();
    }

    /// <summary>
    /// Elimina todo el historial de ubicaciones de una ruta determinada. (Solo Administrador)
    /// </summary>
    [HttpDelete("ruta/{rutaId}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> EliminarPorRuta(int rutaId)
    {
        await _ubicacionService.EliminarHistorialRutaAsync(rutaId);
        return Ok();
    }
}
