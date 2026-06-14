using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Authentication.UploadAvatar;

public sealed record UploadAvatarCommand(
    long FileSize,
    string ContentType,
    Stream FileStream,
    string FileName)
    : IRequest<Result<UploadAvatarResponse>>;