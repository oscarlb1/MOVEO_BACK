using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moveo.Modelos.Configuraciones;

namespace Moveo.Negocio.Servicios;

public class CloudinaryUploadService : IUploadService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryUploadService(IOptions<CloudinarySettings> config)
    {
        var acc = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );

        _cloudinary = new Cloudinary(acc);
    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();

        if (file.Length > 0)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
            };

            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        return uploadResult.SecureUrl?.ToString() ?? string.Empty;
    }
}
