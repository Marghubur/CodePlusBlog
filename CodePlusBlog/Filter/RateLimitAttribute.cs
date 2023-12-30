using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;

namespace CodePlusBlog.Filter
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RateLimitAttribute: ActionFilterAttribute
    {
        private readonly IDistributedCache _cache;

        public RateLimitAttribute(IDistributedCache cache)
        {
            _cache = cache ?? throw new Exception(nameof(cache));
        }

        public override async void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.User.Identity.Name; // Replace this with your actual user identifier logic.

            if (!string.IsNullOrEmpty(userId))
            {
                var cacheKey = $"ratelimit:{userId}";
                var cacheEntry = await _cache.GetStringAsync(cacheKey);

                if (string.IsNullOrEmpty(cacheEntry))
                {
                    // First API call, set initial count and expiration
                    await _cache.SetStringAsync(cacheKey, "1", new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
                    });
                }
                else
                {
                    var count = int.Parse(cacheEntry);

                    if (count >= 3)
                    {
                        // User has exceeded the limit
                        context.Result = new ContentResult
                        {
                            StatusCode = 429, // 429 Too Many Requests
                            Content = "Rate limit exceeded. Try again later."
                        };
                        return;
                    }

                    // Increment the count
                    await _cache.SetStringAsync(cacheKey, (count + 1).ToString());
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
