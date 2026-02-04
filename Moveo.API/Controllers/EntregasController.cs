using Microsoft.AspNetCore.Mvc;
using Moveo.Modelos.DTOs;
using Moveo.Negocio.Servicios;

namespace Moveo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EntregasController : ControllerBase
{
    private readonly IEntregaService _entregaService;

    public EntregasController(IEntregaService entregaService)
    {
        _entregaService = entregaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EntregaDto>>> ObtenerTodas(
        [FromQuery] int? rutaId,
        [FromQuery] int? clienteId,
        [FromQuery] string? estado,
        [FromQuery] DateTime? fecha)
    {
        var entregas = await _entregaService.ObtenerTodasAsync(rutaId, clienteId, estado, fecha);
        return Ok(entregas);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EntregaDto>> ObtenerPorId(int id)
    {
        var entrega = await _entregaService.ObtenerPorIdAsync(id);
        if (entrega == null) return NotFound();
        return Ok(entrega);
    }

    [HttpGet("rutas/{rutaId}")]
    public async Task<ActionResult<IEnumerable<EntregaDto>>> ObtenerPorRuta(int rutaId)
    {
        var entregas = await _entregaService.ObtenerPorRutaAsync(rutaId);
        return Ok(entregas);
    }

    [HttpPost]
    public async Task<ActionResult<EntregaDto>> Crear(CrearEntregaDto crearEntregaDto)
    {
        try
        {
            var entrega = await _entregaService.CrearAsync(crearEntregaDto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = entrega.Id }, entrega);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/estado")]
    public async Task<ActionResult> ActualizarEstado(int id, ActualizarEstadoEntregaDto dto)
    {
        try
        {
            await _entregaService.ActualizarEstadoAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Eliminar(int id)
    {
        await _entregaService.EliminarAsync(id);
        return NoContent();
    }

    [HttpGet("estadisticas/hoy")]
    public async Task<ActionResult<EntregaEstadisticasDto>> ObtenerEstadisticasHoy()
    {
        var estadisticas = await _entregaService.ObtenerEstadisticasDelDiaAsync();
        return Ok(estadisticas);
    }
}
