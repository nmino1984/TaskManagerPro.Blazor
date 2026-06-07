using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TaskManagerPro.Blazor.Application.Common.Interfaces;

namespace TaskManagerPro.Blazor.Infrastructure.Services;

public class CloudinaryPhotoService : IPhotoService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<CloudinaryPhotoService> _logger;

    public CloudinaryPhotoService(IConfiguration configuration, ILogger<CloudinaryPhotoService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> UploadPhotoAsync(Stream fileStream, string fileName, CancellationToken ct)
    {
        var cloudName = _configuration["Cloudinary:CloudName"]
            ?? throw new InvalidOperationException("Cloudinary:CloudName is not configured.");
        var apiKey = _configuration["Cloudinary:ApiKey"]
            ?? throw new InvalidOperationException("Cloudinary:ApiKey is not configured.");
        var apiSecret = _configuration["Cloudinary:ApiSecret"]
            ?? throw new InvalidOperationException("Cloudinary:ApiSecret is not configured.");

        var cloudinary = new Cloudinary(new Account(cloudName, apiKey, apiSecret));
        cloudinary.Api.Secure = true;

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, fileStream),
            PublicId = $"avatars/{fileName}",
            Overwrite = true,
            Transformation = new Transformation().Width(200).Height(200).Crop("fill").Gravity("face")
        };

        var result = await cloudinary.UploadAsync(uploadParams);

        if (result.Error is not null)
        {
            _logger.LogError("Cloudinary upload failed for {FileName}: {Error}", fileName, result.Error.Message);
            throw new InvalidOperationException($"Photo upload failed: {result.Error.Message}");
        }

        return result.SecureUrl.ToString();
    }
}
