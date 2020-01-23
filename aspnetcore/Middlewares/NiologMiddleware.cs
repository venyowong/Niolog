using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Niolog.AspNetCore.Middlewares
{
    public class NiologMiddleware
    {
        private readonly RequestDelegate next;

        public NiologMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await this.next(context);
            NiologManager.FreeLogger();
        }
    }
}