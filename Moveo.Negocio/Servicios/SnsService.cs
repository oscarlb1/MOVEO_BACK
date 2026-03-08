using Amazon;
using Amazon.Runtime; // Necesario para SessionAWSCredentials
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Configuration; // Necesario para leer el appsettings.json
using Moveo.Modelos;

namespace Moveo.Negocio.Servicios
{
    public class SnsService
    {
        private readonly IAmazonSimpleNotificationService _snsClient;
        private const string TopicArn = "arn:aws:sns:us-east-1:406270415122:Moveo-Notificaciones";

        // Inyectamos IConfiguration para leer las llaves del appsettings.json
        public SnsService(IConfiguration configuration)
        {
            var accessKey = configuration["AWS:AccessKey"];
            var secretKey = configuration["AWS:SecretKey"];
            var sessionToken = configuration["AWS:SessionToken"]; // Requerido por AWS Academy

            // Creamos las credenciales de sesión temporales
            var credentials = new SessionAWSCredentials(accessKey, secretKey, sessionToken);

            _snsClient = new AmazonSimpleNotificationServiceClient(
                credentials,
                RegionEndpoint.USEast1
            );
        }

        public async Task<bool> EnviarNotificacionAsync(ContactoRequest datos)
        {
            try
            {
                var cuerpoMensaje = $@"
NUEVA SOLICITUD DE CONTACTO - MOVEO GLOBAL LOGISTICS
════════════════════════════════════════════════════════════════

INFORMACIÓN DEL CLIENTE
────────────────────────────────────────────────────────────────
    Nombre:      {datos.Name}
    Email:       {datos.Email}
    Empresa:     {datos.Company ?? "No indicada"}
    Teléfono:    {datos.Phone ?? "No indicado"}
    Interés:     {datos.Subject.ToUpper()}

CONTENIDO DEL MENSAJE
────────────────────────────────────────────────────────────────
    ""{datos.Message}""

────────────────────────────────────────────────────────────────
Este correo fue generado automáticamente por el sistema Moveo C#.
════════════════════════════════════════════════════════════════";

                var request = new PublishRequest
                {
                    TopicArn = TopicArn,
                    Message = cuerpoMensaje,
                    Subject = $"🚀 Lead Moveo: {datos.Name}"
                };

                var response = await _snsClient.PublishAsync(request);
                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error crítico en SNS Service: {ex.Message}");
                return false;
            }
        }
    }
}