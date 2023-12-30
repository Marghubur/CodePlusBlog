using CodePlusBlog.Model;
using Microsoft.Extensions.Caching.Memory;

namespace CodePlusBlog.CRONService
{
    public class OtpCleanupService : BackgroundService
    {
        private readonly IMemoryCache _cache;
        private readonly List<string> _trackedEntries = new List<string>();
        private readonly object _lockObject = new object();
        public OtpCleanupService(IMemoryCache cache)
        {
            _cache = cache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Run a cleanup task every minute to remove expired OTPs from the cache
            while (!stoppingToken.IsCancellationRequested)
            {
                CleanupExpiredOtps();
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private void CleanupExpiredOtps()
        {
            lock (_lockObject)
            {
                foreach (var userId in _trackedEntries.ToList())
                {
                    if (_cache.TryGetValue<CachedOtpEntry>(userId, out var cachedOtpEntry))
                    {
                        // Check if the OTP has expired
                        if (cachedOtpEntry.ExpirationTime < DateTime.Now)
                        {
                            _cache.Remove(userId);
                            RemoveTrackedEntry(userId);
                        }
                    }
                    else
                    {
                        RemoveTrackedEntry(userId);
                    }
                }
            }
        }

        public void TrackCacheEntry(string userId)
        {
            lock (_lockObject)
            {
                _trackedEntries.Add(userId);
            }
        }

        public void RemoveTrackedEntry(string userId)
        {
            lock (_lockObject)
            {
                _trackedEntries.Remove(userId);
            }
        }
    }
}
