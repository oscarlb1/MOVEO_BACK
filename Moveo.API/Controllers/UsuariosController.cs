using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moveo.Modelos.DTOs;
using Moveo.Negocio.Servicios;
using System.Security.Claims;

namespace Moveo.API.Controllers;

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

    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> ObtenerTodos()
    {
        return Ok(await _userService.ObtenerTodosLosUsuariosAsync());
    }

    [HttpGet("me")]
    public async Task<ActionResult<UsuarioDto>> ObtenerMiPerfil()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _userService.ObtenerUsuarioPorIdAsync(userId);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<UsuarioDto>> ObtenerPorId(int id)
    {
        var user = await _userService.ObtenerUsuarioPorIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<UsuarioDto>> Crear(CrearUsuarioDto createUserDto)
    {
        var user = await _userService.CrearUsuarioAsync(createUserDto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = user.Id }, user);
    }

    [HttpPut("me")]
    public async Task<ActionResult<UsuarioDto>> ActualizarPerfil(ActualizarPerfilDto updateProfileDto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _userService.ActualizarPerfilAsync(userId, updateProfileDto);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<UsuarioDto>> Actualizar(int id, ActualizarUsuarioDto updateUserDto)
    {
        var user = await _userService.ActualizarUsuarioAsync(id, updateUserDto);
        if (user == null) return NotFound();
        return Ok(user);
    }
}
