using AcceptanceTests.Common.Configuration;
using BookingQueueSubscriber.AcceptanceTests.Configuration;
using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Common.Security;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BookingQueueSubscriber.AcceptanceTests.Hooks
{
    public class Setup
    {
        private readonly TestContext _context;
        private readonly IConfigurationRoot _configRoot;

        public Setup()
        {
            _context = new TestContext();
            _configRoot = ConfigurationManager.BuildConfig("F6705640-D918-4180-B98A-BAB7ADAA4817");
            _context.Config = new Config();
            _context.Tokens = new Tokens();
        }

        public TestContext RegisterSecrets()
        {
            var services = RegisterServices();
            RegisterUsernameStem();
            var azureOptions = RegisterAzureSecrets();
            GenerateBearerTokens(azureOptions.Value, services);
            return _context;
        }

        private IOptions<AzureAdConfiguration> RegisterAzureSecrets()
        {
            var azureOptions = Options.Create(_configRoot.GetSection("AzureAd").Get<AzureAdConfiguration>());
            _context.Config.AzureAdConfiguration = azureOptions.Value;
            _context.Config.AzureAdConfiguration.Authority.Should().NotBeNullOrEmpty();
            _context.Config.AzureAdConfiguration.ClientId.Should().NotBeNullOrEmpty();
            _context.Config.AzureAdConfiguration.ClientSecret.Should().NotBeNullOrEmpty();
            _context.Config.AzureAdConfiguration.TenantId.Should().NotBeNullOrEmpty();
            return azureOptions;
        }

        private IOptions<ServicesConfiguration> RegisterServices()
        {
            var services = Options.Create(_configRoot.GetSection("Services").Get<ServicesConfiguration>());
            _context.Config.Services = services.Value;
            _context.Config.Services.BookingsApiUrl.Should().NotBeNullOrEmpty();
            _context.Config.Services.VideoApiUrl.Should().NotBeNullOrEmpty();
            return services;
        }

        private void RegisterUsernameStem()
        {
            _context.Config.UsernameStem = _configRoot.GetValue<string>("UsernameStem");
            _context.Config.UsernameStem.Should().NotBeNullOrEmpty();
        }

        private void GenerateBearerTokens(AzureAdConfiguration azureOptions, IOptions<ServicesConfiguration> services)
        {
            _context.Tokens.BookingsApiBearerToken = new AzureTokenProvider(azureOptions).GetClientAccessToken(
                azureOptions.ClientId, azureOptions.ClientSecret,
                services.Value.BookingsApiUrl.TrimEnd('/'));

            _context.Tokens.VideoApiBearerToken = new AzureTokenProvider(azureOptions).GetClientAccessToken(
                azureOptions.ClientId, azureOptions.ClientSecret,
                services.Value.VideoApiUrl.TrimEnd('/'));

            _context.Tokens.BookingsApiBearerToken.Should().NotBeNullOrEmpty();
            _context.Tokens.VideoApiBearerToken.Should().NotBeNullOrEmpty();
        }
    }
}