using Microsoft.AspNetCore.Mvc;
using Moveo.Modelos.DTOs;
using Moveo.Negocio.Servicios;

namespace Moveo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiculosController : ControllerBase
{
    private readonly IVehiculoService _vehiculoService;

    public VehiculosController(IVehiculoService vehiculoService)
    {
        _vehiculoService = vehiculoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehiculoDto>>> ObtenerTodos()
    {
        var vehiculos = await _vehiculoService.ObtenerTodosLosVehiculosAsync();
        return Ok(vehiculos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VehiculoDto>> ObtenerPorId(int id)
    {
        var vehiculo = await _vehiculoService.ObtenerVehiculoPorIdAsync(id);
        if (vehiculo == null) return NotFound();
        return Ok(vehiculo);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<VehiculoDto>> Actualizar(int id, UpdateVehiculoDto updateVehiculoDto)
    {
        var vehiculo = await _vehiculoService.ActualizarVehiculoAsync(id, updateVehiculoDto);
        if (vehiculo == null) return NotFound();
        return Ok(vehiculo);
    }
}
