using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace RateLimiting.Extensions
{
    public static class RateLimitingExtension
    {
        public static void AddRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                /*
                 Take 10 request if exceed in one hour then it throw 429
                 After every 1 hour 1 request restore and we can consume it
                 */
                options.AddTokenBucketLimiter("TokenBucketPolicy", opt =>
                {
                    opt.TokenLimit = 10;
                    opt.ReplenishmentPeriod = TimeSpan.FromHours(1);
                    opt.TokensPerPeriod = 1;
                    opt.AutoReplenishment = true;
                });

                /*
                    Each time window is divided into multiple segments
                    The window slides one segment each segment interval
                    The segment interval is (window_time)/(segments_per_window)
                    When a segment expires, the requests taken in that segment are added to the current segment
                 */
                options.AddSlidingWindowLimiter("SlidingWindowPolicy", opt =>
                {
                    opt.QueueLimit = 0;
                    opt.PermitLimit = 10;
                    opt.AutoReplenishment = true;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.Window = TimeSpan.FromSeconds(10);
                    opt.SegmentsPerWindow = 2;
                });

                /*
                    The Window value determines the time window.
                 */
                options.AddFixedWindowLimiter("FixedWindowPolicy", opt =>
                {
                    opt.QueueLimit = 0;
                    opt.PermitLimit = 10;
                    opt.AutoReplenishment = true;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.Window = TimeSpan.FromSeconds(10);
                });

                /*
                    The concurrency limiter is the most straightforward algorithm, and it just limits the number of concurrent requests.
                 */
                options.AddConcurrencyLimiter("ConcurrentPolicy", opt =>
                {
                    opt.QueueLimit = 0;
                    opt.PermitLimit = 1;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });
            });
        }
    }
}
