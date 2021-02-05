using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder
{
  public static class EndpointExtensions
  {
    public static void MapEndpoint<T>(this IEndpointRouteBuilder app, string path, string methodName = "Endpoint")
    {
      MethodInfo methodInfo = typeof(T).GetMethod(methodName);
      if (methodInfo == null || methodInfo.ReturnType != typeof(Task))
      {
        throw new System.Exception("Method cannot be used");
      }

      // T endpointInstance = ActivatorUtilities.CreateInstance<T>(app.ServiceProvider);

      // app.MapGet(path, (RequestDelegate)methodInfo.CreateDelegate(typeof(RequestDelegate), endpointInstance));
      ParameterInfo[] methodParams = methodInfo.GetParameters();

      app.MapGet(path, context =>
      // (Task)methodInfo.Invoke(
      //     endpointInstance,
      //     methodParams.Select(p => p.ParameterType == typeof(HttpContext)
      //         ? context
      //         // : app.ServiceProvider.GetService(p.ParameterType)).ToArray() // Root services => not provide access to scoped services
      //         : context.RequestServices.GetService(p.ParameterType)).ToArray()
      // ));
      {
        /*
            This approach requires a new instance of the endpoint class to handle each request
            but it ensures that no knowledge of service lifecycles is required.
        */
        T endpointInstance = ActivatorUtilities.CreateInstance<T>(context.RequestServices);
        return (Task)methodInfo.Invoke(endpointInstance,
                  methodParams.Select(p => p.ParameterType == typeof(HttpContext)
                      ? context : context.RequestServices.GetService(p.ParameterType))
                  .ToArray());
      });
    }
  }
}