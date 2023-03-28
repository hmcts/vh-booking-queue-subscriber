using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using BookingQueueSubscriber.Common.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace BookingQueueSubscriber.Common.Security
{
    public interface IAzureTokenProvider
    {
        Task<string> GetClientAccessToken(string clientId, string clientSecret, string clientResource);
        Task<AuthenticationResult> GetAuthorisationResult(string clientId, string clientSecret, string clientResource);
    }

    [ExcludeFromCodeCoverage]
    public class AzureTokenProvider : IAzureTokenProvider
    {
        private readonly AzureAdConfiguration _azureAdConfiguration;

        public AzureTokenProvider(IOptions<AzureAdConfiguration> azureAdConfigurationOptions)
        {
            _azureAdConfiguration = azureAdConfigurationOptions.Value;
        }

        public async Task<string> GetClientAccessToken(string clientId, string clientSecret, string clientResource)
        {
            var result = await GetAuthorisationResult(clientId, clientSecret, clientResource);
            return result.AccessToken;
        }

        /// <summary>
        /// Get the authorisation result from Azure AD
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="clientResource"></param>
        /// <returns></returns>
        public async Task<AuthenticationResult> GetAuthorisationResult(string clientId, string clientSecret,
            string clientResource)
        {
            AuthenticationResult result;
            var authority = $"{_azureAdConfiguration.Authority}{_azureAdConfiguration.TenantId}";
            var app =ConfidentialClientApplicationBuilder.Create(clientId).WithClientSecret(clientSecret)
                .WithAuthority(authority).Build();
            

            try
            {
                result = await app.AcquireTokenForClient(new[] {$"{clientResource}/.default"}).ExecuteAsync();
            }
            catch (MsalServiceException)
            {
                throw new UnauthorizedAccessException();
            }

            return result;
        }
    }
}
