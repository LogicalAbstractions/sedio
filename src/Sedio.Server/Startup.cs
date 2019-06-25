using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sedio.Contracts.Serialization;
using Sedio.Core;
using Sedio.Core.Application;
using Sedio.Ignite;
using Sedio.Logic.Data;
using Sedio.Logic.Execution;
using Sedio.Server.Integration.Serialization;
using Sedio.Web.Application;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using ILogger = Serilog.ILogger;

namespace Sedio.Server
{
    public sealed class Startup
    {
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices(services =>
                {
                    services.AddAutofac();
                })
                .ConfigureLogging(log =>
                {
                    log.Services.AddSingleton<ILogger>(services =>
                    {
                        var hostEnvironment = services.GetRequiredService<IHostEnvironment>();

                        var logger = new LoggerConfiguration()
                            .MinimumLevel.Is(hostEnvironment.IsProduction() ? LogEventLevel.Warning : LogEventLevel.Debug)
                            .WriteTo.RollingFile(Path.Combine(hostEnvironment.ContentRootPath, "Data", "Logs", "sedio-{Date}.log"))
                            .WriteTo.Console()
                            .CreateLogger();

                        return logger;
                    });

                    log.Services.AddSingleton<ILoggerProvider, SerilogLoggerProvider>(services => 
                        new SerilogLoggerProvider(services.GetRequiredService<ILogger>(), false));

                    log.AddFilter<SerilogLoggerProvider>(null, LogLevel.Trace);
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .Build().Run();
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages(options =>
            {
                options.RootDirectory = "/Ui";
            });
            services.AddServerSideBlazor();

            services.AddControllers()
                .AddControllersAsServices()
                .AddNewtonsoftJson();

            services.AddHttpContextAccessor();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.Register(c => new WebApplication(c.Resolve<IHostEnvironment>()))
                .As<IApplication>().SingleInstance();

            builder.RegisterModule(new CoreModule());
            builder.RegisterModule(new IgniteModule());
            builder.RegisterModule(new DataModule());
            builder.RegisterModule(new ExecutionModule());
            builder.RegisterModule(new JsonSerializationModule());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var logger = app.ApplicationServices.GetRequiredService<ILogger>();

            // Initialize out of bounds:
            var igniteProvider = app.ApplicationServices.GetRequiredService<IgniteProvider>();

            Task.Factory.StartNew(() =>
            {
                var ignite = igniteProvider.Ignite;
                logger.Information("Ignite initialized: {Node}",ignite.Name);
            }, TaskCreationOptions.LongRunning);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/Startup");
            });
        }
    }
}
