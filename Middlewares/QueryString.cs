using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Platform.Middlewares
{
    public class QueryStringMw
    {
        private RequestDelegate next;
        public QueryStringMw()
        {
            // do nothing
        }

        public QueryStringMw(RequestDelegate nextDelegate)
        {
            next = nextDelegate;
        }

        public async Task Invoke(HttpContext ctx)
        {
            if (ctx.Request.Method == HttpMethods.Get && ctx.Request.Query["custom"] == "true")
            {
                await ctx.Response.WriteAsync("Class-based Middleware query string \n");
            }

            if (next != null)
            {
                await next(ctx);
            }
        }
    }
}
