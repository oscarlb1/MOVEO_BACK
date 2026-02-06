using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moveo.Modelos.DTOs;
using Moveo.Negocio.Servicios;

namespace Moveo.API.Controllers;

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

    [HttpPost]
    public async Task<IActionResult> Registrar(RegistroUbicacionDto registroDto)
    {
        await _ubicacionService.RegistrarUbicacionAsync(registroDto);
        return Ok();
    }

    [HttpGet("ruta/{rutaId}")]
    public async Task<ActionResult<IEnumerable<UbicacionDto>>> ObtenerPorRuta(int rutaId)
    {
        var historial = await _ubicacionService.ObtenerHistorialPorRutaAsync(rutaId);
        return Ok(historial);
    }

    [HttpGet("ultimo/ruta/{rutaId}")]
    public async Task<ActionResult<UbicacionDto>> ObtenerUltimoPunto(int rutaId)
    {
        var punto = await _ubicacionService.ObtenerUltimaUbicacionAsync(rutaId);
        if (punto == null) return NotFound();
        return Ok(punto);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Actualizar(int id, RegistroUbicacionDto registroDto)
    {
        var success = await _ubicacionService.ActualizarUbicacionAsync(id, registroDto);
        if (!success) return NotFound();
        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var success = await _ubicacionService.EliminarUbicacionAsync(id);
        if (!success) return NotFound();
        return Ok();
    }

    [HttpDelete("ruta/{rutaId}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> EliminarPorRuta(int rutaId)
    {
        await _ubicacionService.EliminarHistorialRutaAsync(rutaId);
        return Ok();
    }
}
