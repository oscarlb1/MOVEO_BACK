using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Moveo.Modelos.DTOs;
using Moveo.Negocio.Servicios;

namespace Moveo.API.Controllers;

/// <summary>
/// Controlador para la gestión de mantenimientos preventivos y correctivos de la flota.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MantenimientosController : ControllerBase
{
    private readonly IMantenimientoService _mantenimientoService;

    public MantenimientosController(IMantenimientoService mantenimientoService)
    {
        _mantenimientoService = mantenimientoService;
    }

    /// <summary>
    /// Obtiene la lista de todos los mantenimientos registrados.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MantenimientoDto>>> ObtenerTodos()
    {
        var mantenimientos = await _mantenimientoService.ObtenerTodosLosMantenimientosAsync();
        return Ok(mantenimientos);
    }

    /// <summary>
    /// Obtiene los detalles de un mantenimiento específico por su ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<MantenimientoDto>> ObtenerPorId(int id)
    {
        var mantenimiento = await _mantenimientoService.ObtenerMantenimientoPorIdAsync(id);
        if (mantenimiento == null) return NotFound();
        return Ok(mantenimiento);
    }

    /// <summary>
    /// Obtiene el historial de mantenimientos vinculados a un vehículo específico.
    /// </summary>
    [HttpGet("vehiculo/{vehiculoId}")]
    public async Task<ActionResult<IEnumerable<MantenimientoDto>>> ObtenerPorVehiculoId(int vehiculoId)
    {
        var mantenimientos = await _mantenimientoService.ObtenerMantenimientosPorVehiculoIdAsync(vehiculoId);
        return Ok(mantenimientos);
    }

    /// <summary>
    /// Registra un nuevo mantenimiento para un vehículo. (Solo Administrador)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<MantenimientoDto>> Crear(CrearMantenimientoDto dto)
    {
        var mantenimiento = await _mantenimientoService.CrearMantenimientoAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = mantenimiento.Id }, mantenimiento);
    }

    /// <summary>
    /// Actualiza la información de un registro de mantenimiento. (Solo Administrador)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult> Actualizar(int id, ActualizarMantenimientoDto dto)
    {
        var resultado = await _mantenimientoService.ActualizarMantenimientoAsync(id, dto);
        if (!resultado) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Elimina un registro de mantenimiento del sistema. (Solo Administrador)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult> Eliminar(int id)
    {
        var resultado = await _mantenimientoService.EliminarMantenimientoAsync(id);
        if (!resultado) return NotFound();
        return NoContent();
    }
}
