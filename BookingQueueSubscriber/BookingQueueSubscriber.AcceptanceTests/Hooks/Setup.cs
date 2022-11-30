using AcceptanceTests.Common.Exceptions;
using BookingQueueSubscriber.AcceptanceTests.Configuration;
using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Common.Security;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ConfigurationManager = AcceptanceTests.Common.Configuration.ConfigurationManager;

namespace BookingQueueSubscriber.AcceptanceTests.Hooks
{
    public class Setup
    {
        private readonly TestContext _context;
        private readonly IConfigurationRoot _configRoot;

        public Setup()
        {
            _context = new TestContext();
            _configRoot = ConfigurationManager.BuildConfig("F6705640-D918-4180-B98A-BAB7ADAA4817", "0A28103E-8F77-4BCB-85A6-2D3279598623");
            _context.Config = new Config();
            _context.Tokens = new Tokens();
        }

        public TestContext RegisterSecrets()
        {
            var azureOptions = RegisterAzureSecrets();
            var services = RegisterServices();
            RegisterUsernameStem();
            GenerateBearerTokens(azureOptions, services);
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
            return azureOptions;
        }

        private IOptions<ServicesConfiguration> RegisterServices()
        {
            var services = GetTargetTestEnvironment() == string.Empty ? 
                Options.Create(_configRoot.GetSection("VhServices").Get<ServicesConfiguration>()) 
                : Options.Create(_configRoot.GetSection($"Testing.{GetTargetTestEnvironment()}.VhServices").Get<ServicesConfiguration>());
            if (services == null && GetTargetTestEnvironment() != string.Empty)
                throw new TestSecretsFileMissingException(GetTargetTestEnvironment());
            _context.Config.Services = services?.Value;
            _context.Config.Services?.BookingsApiUrl.Should().NotBeNullOrEmpty("BookingsApiUrl is set");
            _context.Config.Services?.VideoApiUrl.Should().NotBeNullOrEmpty("VideoApiUrl is set");
            return services;
        }

        private void RegisterUsernameStem()
        {
            _context.Config.UsernameStem = _configRoot.GetValue<string>("UsernameStem");
            _context.Config.UsernameStem.Should().NotBeNullOrEmpty("UsernameStem is set");
        }

        private static string GetTargetTestEnvironment()
        {
            return NUnit.Framework.TestContext.Parameters["TargetTestEnvironment"] ?? string.Empty;
        }

        private void GenerateBearerTokens(IOptions<AzureAdConfiguration> azureOptions, IOptions<ServicesConfiguration> services)
        {
            _context.Tokens.BookingsApiBearerToken = new AzureTokenProvider(azureOptions).GetClientAccessToken(
                azureOptions.Value.ClientId, azureOptions.Value.ClientSecret,
                services.Value.BookingsApiUrl.TrimEnd('/'));

            _context.Tokens.VideoApiBearerToken = new AzureTokenProvider(azureOptions).GetClientAccessToken(
                azureOptions.Value.ClientId, azureOptions.Value.ClientSecret,
                services.Value.VideoApiUrl.TrimEnd('/'));

            _context.Tokens.BookingsApiBearerToken.Should().NotBeNullOrEmpty("BookingsApiBearerToken is set");
            _context.Tokens.VideoApiBearerToken.Should().NotBeNullOrEmpty("VideoApiBearerToken is set");
        }
    }
}