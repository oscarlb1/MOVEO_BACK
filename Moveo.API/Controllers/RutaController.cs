using Microsoft.AspNetCore.Mvc;
using Moveo.Modelos.DTOs;
using Moveo.Negocio.Servicios;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moveo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RutaController : ControllerBase
{
    private readonly IRutaService _rutaService;

    public RutaController(IRutaService rutaService)
    {
        _rutaService = rutaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RutaDto>>> ObtenerTodas()
    {
        var rutas = await _rutaService.ObtenerTodasAsync();
        return Ok(rutas);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RutaDto>> ObtenerPorId(int id)
    {
        var ruta = await _rutaService.ObtenerPorIdAsync(id);
        if (ruta == null) return NotFound();
        return Ok(ruta);
    }

    [HttpPost]
    public async Task<ActionResult<RutaDto>> Crear(CrearRutaDto rutaDto)
    {
        var nuevaRuta = await _rutaService.CrearAsync(rutaDto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaRuta.Id }, nuevaRuta);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Actualizar(int id, ActualizarRutaDto rutaDto)
    {
        var actualizado = await _rutaService.ActualizarAsync(id, rutaDto);
        if (!actualizado) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Eliminar(int id)
    {
        var eliminado = await _rutaService.EliminarAsync(id);
        if (!eliminado) return NotFound();
        return NoContent();
    }
}
