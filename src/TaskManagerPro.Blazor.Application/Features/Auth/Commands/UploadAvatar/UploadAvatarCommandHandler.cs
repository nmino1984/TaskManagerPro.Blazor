using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagerPro.Blazor.Application.Common.Exceptions;
using TaskManagerPro.Blazor.Application.Common.Interfaces;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.Auth.Commands.UploadAvatar;

public class UploadAvatarCommandHandler : IRequestHandler<UploadAvatarCommand, string>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPhotoService _photoService;
    private readonly ILogger<UploadAvatarCommandHandler> _logger;

    public UploadAvatarCommandHandler(IUnitOfWork unitOfWork, IPhotoService photoService, ILogger<UploadAvatarCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _photoService = photoService;
        _logger = logger;
    }

    public async Task<string> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(AppUser), request.UserId);

        // Use a deterministic public_id per user so re-uploads overwrite the old image in Cloudinary
        var publicId = $"user_{request.UserId:N}";
        var url = await _photoService.UploadPhotoAsync(request.FileStream, publicId, cancellationToken);

        user.SetAvatarUrl(url);
        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Avatar updated for user {UserId}", request.UserId);

        return url;
    }
}
