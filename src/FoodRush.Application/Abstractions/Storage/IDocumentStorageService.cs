using FoodRush.Domain.Common;

namespace FoodRush.Application.Abstractions.Storage;

public interface IDocumentStorageService
{
    Task<Result<UploadResponse>> UploadAsync(
        Stream stream,
        string fileName,
        string contentType,
        long length,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(string publicId, CancellationToken cancellationToken = default);

}

public sealed record UploadResponse(string PublicId, string DownloadUrl);