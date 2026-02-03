using Microsoft.AspNetCore.Mvc;
using Moveo.Modelos.DTOs;
using Moveo.Negocio.Servicios;

namespace Moveo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MantenimientosController : ControllerBase
{
    private readonly IMantenimientoService _mantenimientoService;

    public MantenimientosController(IMantenimientoService mantenimientoService)
    {
        _mantenimientoService = mantenimientoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MantenimientoDto>>> ObtenerTodos()
    {
        var mantenimientos = await _mantenimientoService.ObtenerTodosAsync();
        return Ok(mantenimientos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MantenimientoDto>> ObtenerPorId(int id)
    {
        var mantenimiento = await _mantenimientoService.ObtenerPorIdAsync(id);
        if (mantenimiento == null) return NotFound();
        return Ok(mantenimiento);
    }

    [HttpGet("vehiculo/{vehiculoId}")]
    public async Task<ActionResult<IEnumerable<MantenimientoDto>>> ObtenerPorVehiculoId(int vehiculoId)
    {
        var mantenimientos = await _mantenimientoService.ObtenerPorVehiculoIdAsync(vehiculoId);
        return Ok(mantenimientos);
    }

    [HttpPost]
    public async Task<ActionResult<MantenimientoDto>> Crear(CrearMantenimientoDto dto)
    {
        var mantenimiento = await _mantenimientoService.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = mantenimiento.Id }, mantenimiento);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Actualizar(int id, ActualizarMantenimientoDto dto)
    {
        var resultado = await _mantenimientoService.ActualizarAsync(id, dto);
        if (!resultado) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Eliminar(int id)
    {
        var resultado = await _mantenimientoService.EliminarAsync(id);
        if (!resultado) return NotFound();
        return NoContent();
    }
}
