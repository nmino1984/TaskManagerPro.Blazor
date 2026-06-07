using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.Auth.Commands.UploadAvatar;

public record UploadAvatarCommand(Stream FileStream, string FileName, Guid UserId) : IRequest<string>;
