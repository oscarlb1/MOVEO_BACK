using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration; // Necesario para la inyección de configuración
using Moveo.Modelos;
using Moveo.Negocio.Servicios;

namespace Moveo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactoController : ControllerBase
    {
        private readonly SnsService _snsService;

        // Inyectamos IConfiguration en el constructor
        public ContactoController(IConfiguration configuration)
        {
            // Pasamos la configuración al constructor de SnsService
            _snsService = new SnsService(configuration);
        }

        [HttpPost]
        public async Task<IActionResult> Enviar([FromBody] ContactoRequest request)
        {
            if (request == null)
                return BadRequest(new { mensaje = "Cuerpo de la petición vacío." });

            // Disparamos el proceso en la capa de Negocio
            bool resultado = await _snsService.EnviarNotificacionAsync(request);

            if (resultado)
            {
                return Ok(new { mensaje = "Email enviado correctamente a través de SNS." });
            }

            return StatusCode(500, new { mensaje = "Hubo un problema al conectar con AWS. Revisa las credenciales de Academy." });
        }
    }
}