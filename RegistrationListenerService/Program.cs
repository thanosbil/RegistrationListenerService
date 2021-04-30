using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RegistrationListenerService.Core.DataAccess;
using RegistrationListenerService.Core.Interfaces;
using RegistrationListenerService.Core.Services;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegistrationListenerService {
    public class Program {
        public static void Main(string[] args) {

            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            // Serilog initialization and configuration
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            try {
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();                
            }
            catch (Exception ex) {
                Log.Fatal(ex, "Host terminated unexpectedly");             
            }
            finally {
                Log.CloseAndFlush();
            }
        }
                
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureHostConfiguration(configuration =>
                {
                    configuration.AddJsonFile("appsettings.json", false, true);
                })
                .ConfigureServices((hostContext, services) => {
                    services.AddHostedService<Worker>();
                    
                    // database context
                    services.AddDbContext<RegistrationsDBContext>(options => {
                        options.UseSqlServer(hostContext.Configuration.GetConnectionString("Default"));
                    });

                    // configuration options for the worker service
                    var workerOptions = new WorkerOptions();
                    hostContext.Configuration.Bind(nameof(WorkerOptions), workerOptions);
                    services.AddSingleton(workerOptions);

                    // The service polling the queue
                    services.AddSingleton<IRegistrationPollingService, RegistrationsPollingService>();                    
                });
    }
}
