using Microsoft.AspNetCore.Mvc;
using Moveo.Modelos.DTOs;
using Moveo.Negocio.Servicios;

namespace Moveo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HistorialUbicacionController : ControllerBase
{
    private readonly IHistorialUbicacionService _historialUbicacionService;

    public HistorialUbicacionController(IHistorialUbicacionService historialUbicacionService)
    {
        _historialUbicacionService = historialUbicacionService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HistorialUbicacionDto>>> ObtenerTodos()
    {
        return Ok(await _historialUbicacionService.ObtenerTodosAsync());
    }

    [HttpGet("ruta/{rutaId}")]
    public async Task<ActionResult<IEnumerable<HistorialUbicacionDto>>> ObtenerPorRutaId(int rutaId)
    {
        return Ok(await _historialUbicacionService.ObtenerPorRutaIdAsync(rutaId));
    }

    [HttpPost]
    public async Task<ActionResult<HistorialUbicacionDto>> Crear(CrearHistorialUbicacionDto dto)
    {
        var historial = await _historialUbicacionService.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerTodos), new { id = historial.Id }, historial);
    }
}
