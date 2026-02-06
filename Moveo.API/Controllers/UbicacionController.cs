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
}
