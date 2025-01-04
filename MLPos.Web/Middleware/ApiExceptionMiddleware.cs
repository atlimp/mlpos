using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using MLPos.Core.Exceptions;
using MLPos.Web.Models;
using System.Threading.Tasks;

namespace MLPos.Web.Middleware
{
    public class ApiExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (!httpContext.Request.Path.Value.Contains("/api/"))
            {
                await _next(httpContext);
                return;
            }

            try
            {
                await _next(httpContext);
            }
            catch (EntityNotFoundException e)
            {
                httpContext.Response.StatusCode = 404;
                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsJsonAsync(new BaseError() { Message = e.Message });
            }
            catch (Exception e)
            {
                httpContext.Response.StatusCode = 500;
                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsJsonAsync(new BaseError() { Message = e.Message });
            }
        }
    }
}
