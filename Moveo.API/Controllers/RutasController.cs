using Microsoft.AspNetCore.Mvc;
using Moveo.Modelos.DTOs;
using Moveo.Negocio.Servicios;

namespace Moveo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RutasController : ControllerBase
{
    private readonly IRutaService _rutaService;

    public RutasController(IRutaService rutaService)
    {
        _rutaService = rutaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RutaDto>>> ObtenerTodos()
    {
        return Ok(await _rutaService.ObtenerTodosAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RutaDto>> ObtenerPorId(int id)
    {
        var ruta = await _rutaService.ObtenerPorIdAsync(id);
        if (ruta == null) return NotFound();
        return Ok(ruta);
    }

    [HttpPost]
    public async Task<ActionResult<RutaDto>> Crear(CrearRutaDto dto)
    {
        var ruta = await _rutaService.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = ruta.Id }, ruta);
    }
}
