using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moveo.Modelos.DTOs;
using Moveo.Negocio.Servicios;

namespace Moveo.API.Controllers;

/// <summary>
/// Controlador para la gestión de la flota de vehículos de la empresa.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VehiculosController : ControllerBase
{
    private readonly IVehiculoService _vehiculoService;

    public VehiculosController(IVehiculoService vehiculoService)
    {
        _vehiculoService = vehiculoService;
    }

    /// <summary>
    /// Obtiene la lista de todos los vehículos activos registrados.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehiculoDto>>> ObtenerTodos()
    {
        var vehiculos = await _vehiculoService.ObtenerTodosLosVehiculosAsync();
        return Ok(vehiculos);
    }

    /// <summary>
    /// Obtiene los detalles de un vehículo específico por su ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<VehiculoDto>> ObtenerPorId(int id)
    {
        var vehiculo = await _vehiculoService.ObtenerVehiculoPorIdAsync(id);
        if (vehiculo == null) return NotFound();
        return Ok(vehiculo);
    }

    /// <summary>
    /// Registra un nuevo vehículo en la flota (Solo Administrador).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<VehiculoDto>> Crear(CreateVehiculoDto createVehiculoDto)
    {
        var vehiculo = await _vehiculoService.CrearVehiculoAsync(createVehiculoDto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = vehiculo.Id }, vehiculo);
    }

    /// <summary>
    /// Actualiza la información técnica o administrativa de un vehículo (Solo Administrador).
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<VehiculoDto>> Actualizar(int id, UpdateVehiculoDto updateVehiculoDto)
    {
        var vehiculo = await _vehiculoService.ActualizarVehiculoAsync(id, updateVehiculoDto);
        if (vehiculo == null) return NotFound();
        return Ok(vehiculo);
    }

    /// <summary>
    /// Elimina un vehículo del sistema por su ID (Solo Administrador).
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var result = await _vehiculoService.EliminarVehiculoAsync(id);
        if (!result) return NotFound();
        return Ok(new { message = "Vehículo eliminado correctamente" });
    }
}
