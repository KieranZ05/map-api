using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace MapAPI.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string API_KEY_HEADER = "X-Api-Key";
        private readonly string _requiredApiKey;

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _requiredApiKey = configuration["ApiKeys:FS_ReadWrite"]; // Get the API key from appsettings.json
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (!httpContext.Request.Headers.ContainsKey(API_KEY_HEADER) ||
                httpContext.Request.Headers[API_KEY_HEADER] != _requiredApiKey)
            {
                httpContext.Response.StatusCode = 401; // Unauthorized
                await httpContext.Response.WriteAsync("Unauthorized request.");
                return;
            }

            await _next(httpContext);
        }
    }
}
