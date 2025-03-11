using Azure.Monitor.OpenTelemetry.AspNetCore;
using BookingQueueSubscriber.Health;
using BookingQueueSubscriber.Security;
using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.UserApi;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using NotificationApi.Client;
using UserApi.Client;
using VideoApi.Client;
using BookingsApi.Client;
using Microsoft.Extensions.Configuration.KeyPerFile;
using Microsoft.Extensions.FileProviders;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

[assembly: FunctionsStartup(typeof(Startup))]
namespace BookingQueueSubscriber
{
    [ExcludeFromCodeCoverage]
    public class Startup : FunctionsStartup
    {
        const string VhInfraCore = "vh-infra-core";
        const string VhBookingQueue = "vh-booking-queue";
        const string VhAdminWeb = "vh-admin-web";
        const string VhBookingsApi = "vh-bookings-api";
        const string VhVideoApi = "vh-video-api";
        const string VhNotificationApi = "vh-notification-api";
        const string VhUserApi = "vh-user-api";
        const string VhVideoWeb = "vh-video-web";
        
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var context = builder.GetContext();
            var configBuilder = builder.ConfigurationBuilder
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.json"), true)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.{context.EnvironmentName}.json"),
                    true);

            var keyVaults = new[]
            {
                VhInfraCore, VhBookingQueue, VhAdminWeb, VhBookingsApi, VhVideoApi, VhNotificationApi, VhUserApi,
                VhVideoWeb
            };
            foreach (var keyVault in keyVaults)
            {
                var filePath = $"/mnt/secrets/{keyVault}";
                if (Directory.Exists(filePath))
                {
                    configBuilder.Add(GetKeyPerFileSource(filePath));
                }
            }

            configBuilder
                .AddUserSecrets("F6705640-D918-4180-B98A-BAB7ADAA4817")
                .AddEnvironmentVariables();

            base.ConfigureAppConfiguration(builder);
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var context = builder.GetContext();
            RegisterServices(builder.Services, context.Configuration);
        }

        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            var memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
            services.AddSingleton<IMemoryCache>(memoryCache);
            services.AddHttpContextAccessor();
            services.Configure<AzureAdConfiguration>(options =>
            {
                configuration.GetSection("AzureAd").Bind(options);
            });
            services.Configure<ServicesConfiguration>(options =>
            {
                configuration.GetSection("VhServices").Bind(options);
            });

            var serviceConfiguration = new ServicesConfiguration();
            configuration.GetSection("VhServices").Bind(serviceConfiguration);

            services.AddScoped<IAzureTokenProvider, AzureTokenProvider>();
            services.AddScoped<IMessageHandlerFactory, MessageHandlerFactory>();
            services.AddTransient<VideoServiceTokenHandler>();
            services.AddTransient<VideoWebTokenHandler>();
            services.AddTransient<BookingsServiceTokenHandler>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<NotificationServiceTokenHandler>();
            services.AddTransient<UserServiceTokenHandler>();
            var instrumentationKey = configuration["ApplicationInsights:InstrumentationKey"];
            if(String.IsNullOrWhiteSpace(instrumentationKey))
                Console.WriteLine("Application Insights Instrumentation Key not found");
            else
                services.AddOpenTelemetry()
                    .ConfigureResource(r =>
                    {
                        r.AddService(VhBookingQueue)
                            .AddTelemetrySdk()
                            .AddAttributes(new Dictionary<string, object>
                                { ["service.instance.id"] = Environment.MachineName });
                    })
                    .UseAzureMonitor(options => options.ConnectionString = instrumentationKey) 
                    .WithMetrics()
                    .WithTracing(tracerProvider =>
                    {
                        tracerProvider
                            .AddAspNetCoreInstrumentation(options => options.RecordException = true)
                            .AddHttpClientInstrumentation(options => options.RecordException = true);
                    });
            
            RegisterMessageHandlers(services);

            var container = services.BuildServiceProvider();

            if (serviceConfiguration.EnableVideoApiStub)
            {
                services.AddScoped<IVideoApiService, VideoApiServiceFake>();
                services.AddScoped<IVideoWebService, VideoWebServiceFake>();
                services.AddScoped<INotificationApiClient, NotificationApiClientFake>();
                services.AddScoped<IUserService, UserServiceFake>();
                services.AddScoped<IBookingsApiClient, BookingsApiClientFake>();
                services.AddScoped<ILogger, FakeLogger>();
            }
            else
            {
                services.AddScoped<IVideoApiService, VideoApiService>();
                services.AddHttpClient<IVideoApiClient, VideoApiClient>()
                    .AddHttpMessageHandler(() => container.GetService<VideoServiceTokenHandler>())
                    .AddTypedClient(httpClient =>
                    {
                        var client = VideoApiClient.GetClient(httpClient);
                        client.BaseUrl = serviceConfiguration.VideoApiUrl;
                        client.ReadResponseAsString = true;
                        return (IVideoApiClient)client;
                    });

                services.AddTransient<IVideoWebService, VideoWebService>();
                services.AddHttpClient<IVideoWebService, VideoWebService>(client =>
                {
                    client.BaseAddress = new Uri(serviceConfiguration.VideoWebUrl);
                }).AddHttpMessageHandler(() => container.GetService<VideoWebTokenHandler>());

                services.AddTransient<IUserApiClient, UserApiClient>();
                services.AddHttpClient<IUserApiClient, UserApiClient>()
                    .AddHttpMessageHandler<UserServiceTokenHandler>()
                    .AddTypedClient(httpClient =>
                    {
                        var client = UserApiClient.GetClient(httpClient);
                        client.BaseUrl = serviceConfiguration.UserApiUrl;
                        client.ReadResponseAsString = true;
                        return (IUserApiClient)client;
                    });

                services.AddHttpClient<INotificationApiClient, NotificationApiClient>()
                    .AddHttpMessageHandler<NotificationServiceTokenHandler>()
                    .AddTypedClient(httpClient =>
                    {
                        var client = NotificationApiClient.GetClient(httpClient);
                        client.BaseUrl = serviceConfiguration.NotificationApiUrl;
                        client.ReadResponseAsString = true;
                        return (INotificationApiClient)client;
                    });
                 services.AddHttpClient<IBookingsApiClient, BookingsApiClient>()
                .AddHttpMessageHandler<BookingsServiceTokenHandler>()
                .AddTypedClient(httpClient =>
                {
                    var client = BookingsApiClient.GetClient(httpClient);
                    client.BaseUrl = serviceConfiguration.BookingsApiUrl;
                    client.ReadResponseAsString = true;
                    return (IBookingsApiClient)client;
                });
            }

            services.AddVhHealthChecks();
        }

        private static void RegisterMessageHandlers(IServiceCollection serviceCollection)
        {
            var messageHandlers = GetAllTypesOf(typeof(IMessageHandler<>)).ToList();
            foreach (var messageHandler in messageHandlers)
            {
                var serviceType = messageHandler.GetInterfaces()[0];
                serviceCollection.AddScoped(serviceType, messageHandler);
            }
        }

        private static IEnumerable<Type> GetAllTypesOf(Type i)
        {
            return i.Assembly.GetTypes().Where(t =>
                t.GetInterfaces().ToList().Exists(x =>
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() == i));
        }
        
        private static KeyPerFileConfigurationSource GetKeyPerFileSource(string filePath)
        {
            return new KeyPerFileConfigurationSource
            {
                FileProvider = new PhysicalFileProvider(filePath),
                Optional = true,
                ReloadOnChange = true,
                SectionDelimiter = "--" // Set your custom delimiter here
            };
        }
    }
}