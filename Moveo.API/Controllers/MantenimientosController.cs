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
        return Ok(await _mantenimientoService.ObtenerTodosAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MantenimientoDto>> ObtenerPorId(int id)
    {
        var mantenimiento = await _mantenimientoService.ObtenerPorIdAsync(id);
        if (mantenimiento == null) return NotFound();
        return Ok(mantenimiento);
    }

    [HttpPost]
    public async Task<ActionResult<MantenimientoDto>> Crear(CrearMantenimientoDto dto)
    {
        var mantenimiento = await _mantenimientoService.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = mantenimiento.Id }, mantenimiento);
    }
}
