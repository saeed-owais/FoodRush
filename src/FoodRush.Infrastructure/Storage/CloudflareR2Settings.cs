namespace FoodRush.Infrastructure.Storage;

internal class CloudflareR2Settings
{
    public const string SectionName = "Cloudflare:R2";
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string BucketName { get; set; }
    public string Endpoint { get; set; }
    public string PublicBaseUrl { get; set; }

}
