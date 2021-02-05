using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Platform.Middlewares
{
    public class ConsentMiddleware
    {
        private RequestDelegate next;
        public ConsentMiddleware(RequestDelegate nextDelgate)
        {
            next = nextDelgate;
        }
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == "/consent")
            {
                ITrackingConsentFeature consentFeature = context.Features.Get<ITrackingConsentFeature>();

                if (!consentFeature.HasConsent)
                {
                    consentFeature.GrantConsent();
                }
                else
                {
                    consentFeature.WithdrawConsent();
                }

                await context.Response.WriteAsync(consentFeature.HasConsent ? "Consent Granted \n" : "Consent Withdrawn\n");
            }

            await next(context);
        }
    }
}
