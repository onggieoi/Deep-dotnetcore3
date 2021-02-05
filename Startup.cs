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

using Platform.Endpoints;
using Platform.Models;
using Microsoft.EntityFrameworkCore;

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
      // Cache in memory
      // services.AddDistributedMemoryCache(opts =>
      // {
      //   opts.SizeLimit = 200;
      // });

      // services.AddDistributedSqlServerCache(opts =>
      // {
      //   opts.ConnectionString = Configuration["ConnectionStrings:CalcConnection"];
      //   opts.SchemaName = "dbo";
      //   opts.TableName = "DataCache";
      // });
      // services.AddResponseCaching();

      services.AddDbContext<CalculationContext>(opts =>
      {
        opts.UseSqlServer(Configuration["ConnectionStrings:CalcConnection"]);
        opts.EnableSensitiveDataLogging(true);
      });

      services.AddTransient<SeedData>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
      SeedData seedData, IHostApplicationLifetime lifetime)
    {
      app.UseDeveloperExceptionPage();
      app.UseRouting();
      app.UseResponseCaching();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapEndpoint<SumEndpoint>("/sum/{count:int=1000000000}");

        endpoints.MapGet("/", async context =>
        {
          await context.Response.WriteAsync("Hello World!");
        });
      });

      bool cmdLineInit = (Configuration["INITDB"] ?? "false") == "true";
      if (env.IsDevelopment() || cmdLineInit)
      {
        seedData.SeedDatabase();
        if (cmdLineInit)
        {
          lifetime.StopApplication();
        }
      }
    }
  }
}
