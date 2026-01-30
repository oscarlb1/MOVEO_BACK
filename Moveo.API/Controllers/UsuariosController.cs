using Microsoft.AspNetCore.Mvc;
using Moveo.Modelos.DTOs;
using Moveo.Negocio.Servicios;

namespace Moveo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _userService;

    public UsuariosController(IUsuarioService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> ObtenerTodos()
    {
        return Ok(await _userService.ObtenerTodosLosUsuariosAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UsuarioDto>> ObtenerPorId(int id)
    {
        var user = await _userService.ObtenerUsuarioPorIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioDto>> Crear(CrearUsuarioDto createUserDto)
    {
        var user = await _userService.CrearUsuarioAsync(createUserDto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = user.Id }, user);
    }
}
