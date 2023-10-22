// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Caching.Memory;

var options = new MemoryCacheOptions
{
    ExpirationScanFrequency = TimeSpan.FromMinutes(15)
};
IMemoryCache cache = new MemoryCache(options);
cache.Set("item1", "value1", TimeSpan.FromMinutes(1));
cache.Remove("item1");