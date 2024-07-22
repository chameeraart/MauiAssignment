using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;
using MauiAssignment.Interfaces;
using MauiAssignment.Services;

namespace MauiAssignment
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureSyncfusionCore()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Set up configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Add configuration to builder
            builder.Configuration.AddConfiguration(configuration);

            // Register services with DI
            builder.Services.AddSingleton(sp =>
                new MongoDBService(
                    configuration["MongoDB:ConnectionString"],
                    configuration["MongoDB:DatabaseName"],
                    configuration["MongoDB:CollectionName"]
                ));

            // Register other services as needed
            // builder.Services.AddSingleton<IMyService, MyService>();

            return builder.Build();
        }
    }
}
