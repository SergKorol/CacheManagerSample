using System.Globalization;
using CacheManager.Core;

namespace CacheManagerSample;

static class Program
{
    static async Task Main()
    {
        var cacheManager = new BaseCacheManager<string>(
            new ConfigurationBuilder()
                .WithMicrosoftMemoryCacheHandle()
                .EnablePerformanceCounters()
                .EnableStatistics()
                .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromMinutes(15))
                .Build()
        );

        // Add items to the cache
        cacheManager.Add("item1",
            DateTime.Now.AddMinutes(1).ToString(CultureInfo.InvariantCulture),
            "region1");
        cacheManager.Add("item2",
            DateTime.Now.AddMinutes(2).ToString(CultureInfo.InvariantCulture),
            "region1");
        cacheManager.Add("item1",
            DateTime.Now.AddMinutes(3).ToString(CultureInfo.InvariantCulture), "region2");
        var item = new CacheItem<string>(
            "item2",
            "region2",
            DateTime.Now.AddMinutes(4)
                .ToString(CultureInfo.InvariantCulture),
            ExpirationMode.Absolute,
            TimeSpan.FromSeconds(6));
        cacheManager.Add(item);

        cacheManager.ClearRegion("region1");

        // Start the cache cleanup task (runs in the background)
        await Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                DisplayValue(cacheManager, "item1", "region1");
                DisplayValue(cacheManager, "item2", "region1");
                DisplayValue(cacheManager, "item1", "region2");
                DisplayValue(cacheManager, "item2", "region2");
                await Task.Delay(TimeSpan.FromSeconds(10));
                cacheManager.Clear();
                DisplayValue(cacheManager, "item1", "region1");
                DisplayValue(cacheManager, "item2", "region1");
                DisplayValue(cacheManager, "item1", "region2");
                DisplayValue(cacheManager, "item2", "region2");
            }
        });

        // Continue with other application tasks
        Console.WriteLine("Application is running...");
        Console.ReadLine();
    }
    
    static void DisplayValue(ICacheManager<string> cache, string key, string region)
    {
        var value = cache.Get(key, region);
        Console.WriteLine(value != null
            ? $"Retrieved value for {key} in {region}: {value}"
            : $"Item {key} not found in {region}.");
    }
}