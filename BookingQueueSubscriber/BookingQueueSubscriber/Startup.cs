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

[assembly: FunctionsStartup(typeof(Startup))]
namespace BookingQueueSubscriber
{
    [ExcludeFromCodeCoverage]
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            const string vhInfraCore = "vh-infra-core";
            const string vhBookingQueue = "vh-booking-queue";
            const string vhAdminWeb = "vh-admin-web";
            const string vhBookingsApi = "vh-bookings-api";
            const string vhVideoApi = "vh-video-api";
            const string vhNotificationApi = "vh-notification-api";
            const string vhUserApi = "vh-user-api";
            const string vhVideoWeb = "vh-video-web";

            var context = builder.GetContext();
            var configBuilder = builder.ConfigurationBuilder
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.json"), true)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.{context.EnvironmentName}.json"),
                    true);

            var keyVaults = new[]
            {
                vhInfraCore, vhBookingQueue, vhAdminWeb, vhBookingsApi, vhVideoApi, vhNotificationApi, vhUserApi,
                vhVideoWeb
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
            services.AddSingleton<ITelemetryInitializer>(new CloudRoleNameInitializer());
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
            services.AddApplicationInsightsTelemetryWorkerService();
            
            RegisterMessageHandlers(services);

            var container = services.BuildServiceProvider();

            if (serviceConfiguration.EnableVideoApiStub)
            {
                services.AddScoped<IVideoApiService, VideoApiServiceFake>();
                services.AddScoped<IVideoWebService, VideoWebServiceFake>();
                services.AddScoped<INotificationApiClient, NotificationApiClientFake>();
                services.AddScoped<IUserService, UserServiceFake>();
                services.AddScoped<IBookingsApiClient, BookingsApiClientFake>();
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