namespace TaskManagerPro.Blazor.Application.Common.Interfaces;

/// <summary>
/// Abstracts photo storage so the Application layer has no direct dependency on Cloudinary.
/// </summary>
public interface IPhotoService
{
    Task<string> UploadPhotoAsync(Stream fileStream, string fileName, CancellationToken ct);
}
