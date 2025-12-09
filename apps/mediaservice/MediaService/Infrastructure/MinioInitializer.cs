using Minio;
using Minio.DataModel.Args;

namespace Waggle.MediaService.Infrastructure
{
    public static class MinioInitializer
    {
        public static async Task EnsureBucketsExistAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var minioClient = scope.ServiceProvider.GetRequiredService<IMinioClient>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
                .CreateLogger(typeof(MinioInitializer));

            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var requiredBuckets = config.GetSection("Media:Buckets").Get<string[]>() ?? [];
            var isProduction = config.GetValue("IsProduction", false);

            foreach (var bucket in requiredBuckets)
            {
                try
                {
                    var exists = await minioClient.BucketExistsAsync(
                        new BucketExistsArgs().WithBucket(bucket)
                    );

                    if (!exists)
                    {
                        await minioClient.MakeBucketAsync(
                            new MakeBucketArgs().WithBucket(bucket)
                        );
                        logger.LogInformation("Created MinIO bucket: {Bucket}", bucket);
                    }

                    if (!isProduction)
                    {
                        var policy = $@"{{
                            ""Version"": ""2012-10-17"",
                            ""Statement"": [
                                {{
                                    ""Effect"": ""Allow"",
                                    ""Principal"": {{""AWS"": [""*""]}},
                                    ""Action"": [""s3:GetObject""],
                                    ""Resource"": [""arn:aws:s3:::{bucket}/*""]
                                }}
                            ]
                        }}";

                        await minioClient.SetPolicyAsync(
                            new SetPolicyArgs()
                                .WithBucket(bucket)
                                .WithPolicy(policy)
                        );

                        logger.LogWarning("Set bucket {Bucket} to PUBLIC (development mode only)", bucket);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to ensure bucket exists: {Bucket}", bucket);
                    throw;
                }
            }
        }
    }
}
