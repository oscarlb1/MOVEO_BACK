using Microsoft.AspNetCore.Http;

namespace Moveo.Negocio.Servicios;

public interface IUploadService
{
    Task<string> UploadImageAsync(IFormFile file);
}
