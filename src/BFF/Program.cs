using Katsuretsu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Katsuretsu.BFF
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = GetConfiguration(args);
            Log.Logger = CreateSerilogLogger(configuration);
            try
            {

                Log.Information("Creating the host");

                var host = CreateHostBuilder(args).Build();

                Log.Information("Applying migrations ({ApplicationContext})...", "Katsuretsu.BFF");
                await host.SeedApplicationDataAsync();



                Log.Information("Starting up the host");

                await host.RunAsync();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Host start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var configuration = GetConfiguration(args);
            var host = Host.CreateDefaultBuilder(args);

            host.UseSerilog();

            host.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureKestrel(options => options.AddServerHeader = false);
                webBuilder.UseStartup<Startup>();
            });

            host.ConfigureAppConfiguration(builder =>
            {
                builder.AddConfiguration(configuration);
            });

            return host;
        }



        private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithProperty("ApplicationContext", "RestaurantBFF")
                //.WriteTo.Console()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }


        private static IConfiguration GetConfiguration(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("IdentityServerData.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args);

            return builder.Build();
        }
    }
}
