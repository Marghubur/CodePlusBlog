using CodePlusBlog.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CodePlusBlog.Filter
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RateLimitAttribute : ActionFilterAttribute
    {
        private static readonly Dictionary<string, Tuple<int, DateTime>> UserLimits = new Dictionary<string, Tuple<int, DateTime>>();
        private const int MaxRequests = 3;
        private static readonly TimeSpan RateLimitDuration = TimeSpan.FromHours(24);

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string email = context.ActionArguments["email"].ToString();

            if (!string.IsNullOrEmpty(email))
            {
                if (UserLimits.TryGetValue(email, out var userLimit))
                {
                    var currentTime = DateTime.UtcNow;

                    if (currentTime - userLimit.Item2 < RateLimitDuration)
                    {
                        if (userLimit.Item1 >= MaxRequests)
                        {
                            // User has exceeded the limit
                            var response = new ApiResponse("Rate limit exceeded. Try again later.", (int)StatusCodes.Status429TooManyRequests);
                            context.Result = new ObjectResult(response);
                            return;
                        }

                        // Increment the count
                        UserLimits[email] = new Tuple<int, DateTime>(userLimit.Item1 + 1, userLimit.Item2);
                    }
                    else
                    {
                        // Reset count for a new time window
                        UserLimits[email] = new Tuple<int, DateTime>(1, currentTime);
                    }
                }
                else
                {
                    // First API call for this user
                    UserLimits[email] = new Tuple<int, DateTime>(1, DateTime.UtcNow);
                }
            }

            base.OnActionExecuting(context);
        }

        //private readonly IMemoryCache _cache;

        //public RateLimitAttribute(IMemoryCache cache)
        //{
        //    _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        //}

        //public override void OnActionExecuting(ActionExecutingContext context)
        //{
        //    string email = context.ActionArguments["email"].ToString();

        //    if (!string.IsNullOrEmpty(email))
        //    {
        //        var cacheKey = $"ratelimit:{email}";

        //        if (!_cache.TryGetValue(cacheKey, out int count))
        //        {
        //            // First API call, set initial count and expiration
        //            _cache.Set(cacheKey, 1, new MemoryCacheEntryOptions
        //            {
        //                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        //            });
        //        }
        //        else
        //        {
        //            if (count >= 3)
        //            {
        //                // User has exceeded the limit
        //                var response = new ApiResponse("Rate limit exceeded. Try again later.", (int)StatusCodes.Status429TooManyRequests);
        //                context.Result = new ObjectResult(response);
        //                return;
        //            }

        //            // Increment the count
        //            _cache.Set(cacheKey, count + 1, new MemoryCacheEntryOptions
        //            {
        //                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        //            });
        //        }
        //    }

        //    base.OnActionExecuting(context);
        //}
    }
}
