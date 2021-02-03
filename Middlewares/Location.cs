using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Platform.Middlewares
{
    public class LocationMw
    {
        private RequestDelegate next;
        private MessageOptions options;
        public LocationMw(RequestDelegate nextDelegate, IOptions<MessageOptions> opts)
        {
            next = nextDelegate;
            options = opts.Value;
        }
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == "/location")
            {
                await context.Response
                .WriteAsync($"{options.CityName}, {options.CountryName}");
            }
            else
            {
                await next(context);
            }
        }
    }
}
