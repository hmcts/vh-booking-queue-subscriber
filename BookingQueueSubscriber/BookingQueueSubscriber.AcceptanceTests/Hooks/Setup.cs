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
            _configRoot = ConfigurationManager.BuildConfig("F6705640-D918-4180-B98A-BAB7ADAA4817", GetTargetEnvironment());
            _context.Config = new Config();
            _context.Tokens = new Tokens();
        }

        private static string GetTargetEnvironment()
        {
            return NUnit.Framework.TestContext.Parameters["TargetEnvironment"] ?? "";
        }

        public TestContext RegisterSecrets()
        {
            var azureOptions = RegisterAzureSecrets();
            var services = RegisterServices();
            RegisterUsernameStem();
            GenerateBearerTokens(azureOptions.Value, services);
            return _context;
        }

        private IOptions<AzureAdConfiguration> RegisterAzureSecrets()
        {
            var azureOptions = Options.Create(_configRoot.GetSection("AzureAd").Get<AzureAdConfiguration>());
            _context.Config.AzureAdConfiguration = azureOptions.Value;
            _context.Config.AzureAdConfiguration.Authority.Should().NotBeNullOrEmpty("Authority is set");
            _context.Config.AzureAdConfiguration.ClientId.Should().NotBeNullOrEmpty("ClientId is set");
            _context.Config.AzureAdConfiguration.ClientSecret.Should().NotBeNullOrEmpty("ClientSecret is set");
            _context.Config.AzureAdConfiguration.TenantId.Should().NotBeNullOrEmpty("TenantId is set");
            RemoveTenantIdFromAuthorityIfExists();
            return azureOptions;
        }

        private void RemoveTenantIdFromAuthorityIfExists()
        {
            if (_context.Config.AzureAdConfiguration.Authority.Contains(_context.Config.AzureAdConfiguration.TenantId))
            {
                _context.Config.AzureAdConfiguration.Authority = _context.Config.AzureAdConfiguration.Authority.Replace(_context.Config.AzureAdConfiguration.TenantId, string.Empty);
            }
        }

        private IOptions<ServicesConfiguration> RegisterServices()
        {
            var services = Options.Create(_configRoot.GetSection("VhServices").Get<ServicesConfiguration>());
            _context.Config.Services = services.Value;
            _context.Config.Services.BookingsApiUrl.Should().NotBeNullOrEmpty("BookingsApiUrl is set");
            _context.Config.Services.VideoApiUrl.Should().NotBeNullOrEmpty("VideoApiUrl is set");
            return services;
        }

        private void RegisterUsernameStem()
        {
            _context.Config.UsernameStem = _configRoot.GetValue<string>("UsernameStem");
            _context.Config.UsernameStem.Should().NotBeNullOrEmpty("UsernameStem is set");
        }

        private void GenerateBearerTokens(AzureAdConfiguration azureOptions, IOptions<ServicesConfiguration> services)
        {
            _context.Tokens.BookingsApiBearerToken = new AzureTokenProvider(azureOptions).GetClientAccessToken(
                azureOptions.ClientId, azureOptions.ClientSecret,
                services.Value.BookingsApiUrl.TrimEnd('/'));

            _context.Tokens.VideoApiBearerToken = new AzureTokenProvider(azureOptions).GetClientAccessToken(
                azureOptions.ClientId, azureOptions.ClientSecret,
                services.Value.VideoApiUrl.TrimEnd('/'));

            _context.Tokens.BookingsApiBearerToken.Should().NotBeNullOrEmpty("BookingsApiBearerToken is set");
            _context.Tokens.VideoApiBearerToken.Should().NotBeNullOrEmpty("VideoApiBearerToken is set");
        }
    }
}