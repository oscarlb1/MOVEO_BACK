using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moveo.Modelos.DTOs;
using Moveo.Negocio.Servicios;
using System.Security.Claims;

namespace Moveo.API.Controllers;

/// <summary>
/// Controlador para la gestión de usuarios, roles y perfiles en el sistema.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _userService;

    public UsuariosController(IUsuarioService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Obtiene la lista de todos los usuarios registrados (Solo Administrador).
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> ObtenerTodos()
    {
        return Ok(await _userService.ObtenerTodosLosUsuariosAsync());
    }

    /// <summary>
    /// Obtiene la información del perfil del propio usuario autenticado.
    /// </summary>
    [HttpGet("me")]
    public async Task<ActionResult<UsuarioDto>> ObtenerMiPerfil()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _userService.ObtenerUsuarioPorIdAsync(userId);
        if (user == null) return NotFound();
        return Ok(user);
    }

    /// <summary>
    /// Obtiene la información detallada de un usuario por su ID (Solo Administrador).
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<UsuarioDto>> ObtenerPorId(int id)
    {
        var user = await _userService.ObtenerUsuarioPorIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    /// <summary>
    /// Crea un nuevo usuario en el sistema (Solo Administrador).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<UsuarioDto>> Crear([FromForm] CrearUsuarioDto createUserDto)
    {
        var user = await _userService.CrearUsuarioAsync(createUserDto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = user.Id }, user);
    }

    /// <summary>
    /// Actualiza el perfil del usuario autenticado.
    /// </summary>
    [HttpPut("me")]
    public async Task<ActionResult<UsuarioDto>> ActualizarPerfil([FromForm] ActualizarPerfilDto updateProfileDto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _userService.ActualizarPerfilAsync(userId, updateProfileDto);
        if (user == null) return NotFound();
        return Ok(user);
    }

    /// <summary>
    /// Actualiza la información de un usuario específico por su ID (Solo Administrador).
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<UsuarioDto>> Actualizar(int id, [FromForm] ActualizarUsuarioDto updateUserDto)
    {
        var user = await _userService.ActualizarUsuarioAsync(id, updateUserDto);
        if (user == null) return NotFound();
        return Ok(user);
    }

    /// <summary>
    /// Elimina un usuario del sistema por su ID (Solo Administrador).
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var result = await _userService.EliminarUsuarioAsync(id);
        if (!result) return NotFound();
        return Ok(new { message = "Usuario eliminado correctamente" });
    }
}
