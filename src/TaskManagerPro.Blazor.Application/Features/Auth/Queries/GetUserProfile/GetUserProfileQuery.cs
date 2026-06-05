using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.Auth.Queries.GetUserProfile;

public record GetUserProfileQuery(Guid UserId) : IRequest<UserProfileDto>;
