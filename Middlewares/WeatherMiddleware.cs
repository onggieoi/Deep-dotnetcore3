using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Platform.Services;

namespace Platform.Middlewares
{
    public class WeatherMiddleware
    {
        private RequestDelegate next;
        // private IResponseFormatter formatter;

        public WeatherMiddleware(RequestDelegate nextRq)
        {
            next = nextRq;
            // formatter = resFormatter;
        }

        public async Task Invoke(HttpContext context,
            IResponseFormatter formatter1, IResponseFormatter formatter2, IResponseFormatter formatter3)
        {
            if (context.Request.Path == "/middleware/class")
            {
                // await formatter.Format(context, "Middleware class: It is raining in london");
                await formatter1.Format(context, string.Empty);
                await formatter2.Format(context, string.Empty);
                await formatter3.Format(context, string.Empty);
            }
            else
            {
                await next(context);
            }
        }
    }
}
