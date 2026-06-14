using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FoodRush.Application.Abstractions.Storage;
using Microsoft.Extensions.Options;

namespace FoodRush.Infrastructure.Storage;

internal sealed class CloudinaryFileStorageService : IFileStorageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryFileStorageService(IOptions<CloudinarySettings> options)
    {
        CloudinarySettings settings = options.Value;

        Account account = new(
            settings.CloudName,
            settings.ApiKey,
            settings.ApiSecret);

        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadImageAsync(Stream stream, string publicId, CancellationToken cancellationToken = default)
    {
        ImageUploadParams uploadParams = new()
        {
            File = new FileDescription(publicId, stream),
            PublicId = publicId,
            Overwrite = true,
            Folder = "foodrush"
        };

        ImageUploadResult result =
            await _cloudinary.UploadAsync(uploadParams);

        if (result.Error is not null)
        {
            throw new InvalidOperationException(
                result.Error.Message);
        }

        return result.SecureUrl.ToString();
    }
}