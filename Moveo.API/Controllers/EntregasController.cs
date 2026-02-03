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
    public async Task<ActionResult<IEnumerable<EntregaDto>>> ObtenerTodos()
    {
        return Ok(await _entregaService.ObtenerTodosAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EntregaDto>> ObtenerPorId(int id)
    {
        var entrega = await _entregaService.ObtenerPorIdAsync(id);
        if (entrega == null) return NotFound();
        return Ok(entrega);
    }

    [HttpPost]
    public async Task<ActionResult<EntregaDto>> Crear(CrearEntregaDto dto)
    {
        var entrega = await _entregaService.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = entrega.Id }, entrega);
    }
}
