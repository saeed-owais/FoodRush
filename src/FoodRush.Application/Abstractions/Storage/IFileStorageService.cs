namespace FoodRush.Application.Abstractions.Storage;

public interface IFileStorageService
{
    Task<string> UploadImageAsync(
        Stream stream,
        string fileName,
        CancellationToken cancellationToken);
}