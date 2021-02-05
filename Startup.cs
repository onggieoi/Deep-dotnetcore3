using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

using Platform.Middlewares;
using Microsoft.AspNetCore.HostFiltering;

namespace Platform
{
  public class Startup
  {
    private IConfiguration Configuration { get; set; }
    public Startup(IConfiguration config)
    {
      Configuration = config;
    }
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      // 
      // services.Configure<CookiePolicyOptions>(opts =>
      // {
      //   opts.CheckConsentNeeded = context => true;
      // });

      services.AddDistributedMemoryCache();
      services.AddSession(options =>
      {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.IsEssential = true;
      });

      // configure HostFiltering
      services.Configure<HostFilteringOptions>(opts =>
      {
        // remove the wildcard entry that has been loaded from the appsettings.json file
        opts.AllowedHosts.Clear();
        opts.AllowedHosts.Add("*.example.com");
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseDeveloperExceptionPage();
      // The EU General Data Protection Regulation(GDPR)
      app.UseCookiePolicy();
      app.UseMiddleware<ConsentMiddleware>();

      app.UseSession();
      app.UseRouting();

      // app.Use(async (context, next) =>
      // {
      //     string defaultDebug = Configuration["Logging:LogLevel:Default"];
      //     await context.Response.WriteAsync($"The config setting is: {defaultDebug}");
      // });

      // map to wwwroot
      app.UseStaticFiles();

      // get static file from /files/static.html map to /staticfiles
      // app.UseStaticFiles(new StaticFileOptions
      // {
      //     FileProvider = new PhysicalFileProvider($"{env.ContentRootPath}/staticfiles"),
      //     RequestPath = "/files"
      // });

      app.UseEndpoints(endpoints =>
      {
        // endpoints.MapGet("/cookie", async context =>
        //       {
        //         int counter1 = int.Parse(context.Request.Cookies["counter1"] ?? "0") + 1;
        //         context.Response.Cookies.Append("counter1", counter1.ToString(),
        //               new CookieOptions
        //               {
        //                 MaxAge = TimeSpan.FromMinutes(30),
        //                 // check policy => true to use
        //                 // IsEssential = true,
        //               });

        //         int counter2 = int.Parse(context.Request.Cookies["counter2"] ?? "0") + 1;
        //         context.Response.Cookies.Append("counter2", counter1.ToString(),
        //               new CookieOptions
        //               {
        //                 MaxAge = TimeSpan.FromMinutes(30),
        //               });

        //         await context.Response.WriteAsync($"Counter1: {counter1}, Counter2: {counter2}");
        //       });

        // endpoints.MapGet("clear", context =>
        //       {
        //         context.Response.Cookies.Delete("counter1");
        //         context.Response.Cookies.Delete("counter2");
        //         context.Response.Redirect("/");
        //         return Task.CompletedTask;
        //       });
        endpoints.MapGet("/session", async context =>
        {
          int counter1 = (context.Session.GetInt32("counter1") ?? 0) + 1;
          int counter2 = (context.Session.GetInt32("counter2") ?? 0) + 1;
          context.Session.SetInt32("counter1", counter1);
          context.Session.SetInt32("counter2", counter2);
          await context.Session.CommitAsync();
          await context.Response.WriteAsync($"Counter1: {counter1}, Counter2: {counter2}");
        });

        endpoints.MapFallback(async context => await context.Response.WriteAsync("Hello World!"));
      });
    }
  }
}
