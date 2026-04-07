using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;

namespace UEntity.MongoDb;

/// <summary>
/// Extension methods for configuring MongoDB services.
/// </summary>
public static class UEntityMongoDbExtension
{
    public static MongoClient? UEntityMongoClient;

    /// <summary>
    /// Configures MongoDB services and initializes the client.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="mongoClient">The MongoDB client instance to use.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddUEntityMongoDb([NotNull] this IServiceCollection services, MongoClient mongoClient)
    {
        ArgumentNullException.ThrowIfNull(services);
        UEntityMongoClient = mongoClient;
        DbMonitor();
        return services;
    }

    private static async Task DbMonitor()
    {
        while (true)
        {
            try
            {
                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                await UEntityMongoClient!.GetDatabase("admin").RunCommandAsync<BsonDocument>(new BsonDocument { { "ping", 1 } },
                    cancellationToken: cancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n{DateTime.Now.ToString("u")} MongoDB connection failed: {e.Message}");

                try
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{DateTime.Now.ToString("u")} Re-establishing the MongoDB connection...");
                    UEntityMongoClient = new MongoClient(UEntityMongoClient!.Settings);

                    using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                    await UEntityMongoClient!.GetDatabase("admin").RunCommandAsync<BsonDocument>(new BsonDocument { { "ping", 1 } },
                        cancellationToken: cancellationTokenSource.Token);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{DateTime.Now.ToString("u")} The MongoDB connection was successfully re-established.");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{DateTime.Now.ToString("u")} MongoDB reconnection failure: {ex.Message}");
                }

                Console.ResetColor();
            }

            await Task.Delay(TimeSpan.FromSeconds(10));
        }
    }
}