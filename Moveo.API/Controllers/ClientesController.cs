using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Moveo.Modelos.DTOs;
using Moveo.Negocio.Servicios;

namespace Moveo.API.Controllers;

/// <summary>
/// Controlador para la gestión de clientes en el sistema.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    /// <summary>
    /// Obtiene la lista de todos los clientes registrados. (Solo Administrador)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> ObtenerTodos()
    {
        var clientes = await _clienteService.ObtenerTodosAsync();
        return Ok(clientes);
    }

    /// <summary>
    /// Obtiene la información detallada de un cliente por su ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ClienteDto>> ObtenerPorId(int id)
    {
        var cliente = await _clienteService.ObtenerPorIdAsync(id);
        if (cliente == null) return NotFound();
        return Ok(cliente);
    }

    /// <summary>
    /// Registra un nuevo cliente en el sistema. (Solo Administrador)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ClienteDto>> Crear(CrearClienteDto crearClienteDto)
    {
        var cliente = await _clienteService.CrearAsync(crearClienteDto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = cliente.Id }, cliente);
    }

    /// <summary>
    /// Actualiza la información de un cliente existente. (Solo Administrador)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult> Actualizar(int id, CrearClienteDto crearClienteDto)
    {
        try
        {
            await _clienteService.ActualizarAsync(id, crearClienteDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Elimina un cliente del sistema por su ID. (Solo Administrador)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult> Eliminar(int id)
    {
        await _clienteService.EliminarAsync(id);
        return NoContent();
    }
}
