using Amazon.S3;
using Amazon.S3.Model;
using FoodRush.Application.Abstractions.Storage;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Infrastructure.Resilience;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Registry;
using Polly.Timeout;
using System.Net;

namespace FoodRush.Infrastructure.Storage;

internal sealed class CloudflareR2DocumentStorageService
    : IDocumentStorageService
{
    private readonly CloudflareR2Settings _settings;
    private readonly IAmazonS3 _s3Client;
    private readonly ResiliencePipeline _uploadPipeline;
    private readonly ILogger<CloudflareR2DocumentStorageService> _logger;

    public CloudflareR2DocumentStorageService(
        IAmazonS3 s3Client,
        IOptions<CloudflareR2Settings> options,
        ILogger<CloudflareR2DocumentStorageService> logger,
        ResiliencePipelineProvider<string> uploadPipelineProvider)
    {
        _s3Client = s3Client;
        _settings = options.Value;
        _logger = logger;

        _uploadPipeline = uploadPipelineProvider
            .GetPipeline(PipelineNames.R2Upload);
    }

    public async Task<Result<UploadResponse>> UploadAsync(
        Stream stream,
        string fileName,
        string contentType,
        long contentLength,
        CancellationToken cancellationToken = default)
    {
        var key = BuildObjectKey(fileName);

        try
        {
            var request = new PutObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = key,
                InputStream = stream,
                ContentType = contentType,
                AutoCloseStream = false,
                UseChunkEncoding = false
            };

            request.Headers.ContentLength = contentLength;

            var response = await _uploadPipeline.ExecuteAsync(
                async token =>
                {
                    if (stream.CanSeek)
                    {
                        stream.Position = 0;
                    }

                    return await _s3Client.PutObjectAsync(
                        request,
                        token);
                },
                cancellationToken);

            if (response.HttpStatusCode is not
                (HttpStatusCode.OK or HttpStatusCode.NoContent))
            {
                _logger.LogError(
                    "Failed to upload document to Cloudflare R2. Status code: {StatusCode}",
                    response.HttpStatusCode);

                return Result.Failure<UploadResponse>(
                    Error.Problem(
                        "CloudflareR2DocumentStorageService.UploadAsync",
                        $"Failed to upload document. Status code: {response.HttpStatusCode}"));
            }

            return Result.Success(new UploadResponse(key, BuildDownloadUrl(key)));
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(
                "Document upload was canceled for file: {FileName}",
                fileName);

            throw;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to upload file {Key} to bucket {Bucket}",
                key,
                _settings.BucketName);

            return Result.Failure<UploadResponse>(
                Error.Problem(
                    "CloudflareR2DocumentStorageService.UploadAsync",
                    $"Failed to upload document. Status code: {ex.StatusCode}"));
        }
        catch (TimeoutRejectedException ex)
        {
            _logger.LogError(
                ex,
                "Upload timed out for file {FileName}",
                fileName);

            return Result.Failure<UploadResponse>(
                Error.Problem(
                    "CloudflareR2DocumentStorageService.Timeout",
                    "Upload operation timed out."));
        }
    }

    public async Task<Result> DeleteAsync(
        string publicId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(publicId))
        {
            return Result.Failure(
                Error.Problem(
                    "CloudflareR2DocumentStorageService.DeleteAsync",
                    "Invalid public ID."));
        }

        try
        {
            var response = await _s3Client.DeleteObjectAsync(
                _settings.BucketName,
                publicId,
                cancellationToken);

            if (response.HttpStatusCode is
                HttpStatusCode.OK or HttpStatusCode.NoContent)
            {
                return Result.Success();
            }
            else
            {
                _logger.LogError(
                    "Failed to delete document from Cloudflare R2. Status code: {StatusCode}",
                    response.HttpStatusCode);
                return Result.Failure(
                    Error.Problem(
                        "CloudflareR2DocumentStorageService.DeleteAsync",
                        $"Failed to delete document. Status code: {response.HttpStatusCode}"));
            }
        }
        catch (AmazonS3Exception ex)
        {

            _logger.LogError(
                ex,
                "Failed to delete file {Key} from bucket {Bucket}",
                publicId,
                _settings.BucketName);

            return Result.Failure(
                Error.Problem(
                    "CloudflareR2DocumentStorageService.DeleteAsync",
                    "Failed to delete file."));
        }
    }

    private string BuildDownloadUrl(string key)
    {
        if (!string.IsNullOrWhiteSpace(_settings.PublicBaseUrl))
        {
            return $"{_settings.PublicBaseUrl.TrimEnd('/')}/{key}";
        }

        return $"{_settings.Endpoint.TrimEnd('/')}/{_settings.BucketName}/{key}";
    }

    private static string BuildObjectKey(string fileName)
    {
        var sanitized = Sanitize(
            Path.GetFileNameWithoutExtension(fileName));

        var extension = Path.GetExtension(fileName);


        return $"books/{sanitized}-{Guid.NewGuid():N}{extension}";
    }

    private static string Sanitize(string value)
    {
        var invalidChars = Path.GetInvalidFileNameChars();

        var sanitized = new string(
            value
                .Where(c =>
                    !invalidChars.Contains(c) &&
                    (
                        char.IsLetterOrDigit(c)
                        || c == '-'
                        || c == '_'
                    ))
                .ToArray());

        return sanitized.Trim();
    }
}