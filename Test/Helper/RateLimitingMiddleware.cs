using System.Collections.Concurrent;

namespace Test.Helper
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentDictionary<string, (DateTime Timestamp, int Count)> _clients = new();

        private readonly int _maxRequests = 5;
        private readonly TimeSpan _timeWindow = TimeSpan.FromSeconds(10);

        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var now = DateTime.UtcNow;

            _clients.AddOrUpdate(ip, (now, 1), (key, value) =>
            {
                if ((now - value.Timestamp) > _timeWindow)
                    return (now, 1);
                else
                    return (value.Timestamp, value.Count + 1);
            });

            if (_clients[ip].Count > _maxRequests)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Too many requests. Try again later.");
                return;
            }

            await _next(context);
        }
    }
}
