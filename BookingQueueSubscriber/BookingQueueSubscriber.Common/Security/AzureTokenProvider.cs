using System;
using BookingQueueSubscriber.Common.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace BookingQueueSubscriber.Common.Security
{
    public interface IAzureTokenProvider
    {
        string GetClientAccessToken(string clientId, string clientSecret, string clientResource);
        AuthenticationResult GetAuthorisationResult(string clientId, string clientSecret, string clientResource);
    }

    public class AzureTokenProvider : IAzureTokenProvider
    {
        private readonly AzureAdConfiguration _azureAdConfiguration;

        public AzureTokenProvider(AzureAdConfiguration azureAdConfiguration)
        {
            _azureAdConfiguration = azureAdConfiguration;
        }

        public string GetClientAccessToken(string clientId, string clientSecret, string clientResource)
        {
            var result = GetAuthorisationResult(clientId, clientSecret, clientResource);
            return result.AccessToken;
        }

        public AuthenticationResult GetAuthorisationResult(string clientId, string clientSecret, string clientResource)
        {
            AuthenticationResult result;
            var credential = new ClientCredential(clientId, clientSecret);
            var authContext =
                new AuthenticationContext($"{_azureAdConfiguration.Authority.Replace(_azureAdConfiguration.TenantId, string.Empty)}{_azureAdConfiguration.TenantId}");

            try
            {
                result = authContext.AcquireTokenAsync(clientResource, credential).Result;
            }
            catch (AdalException)
            {
                throw new UnauthorizedAccessException();
            }

            return result;
        }
    }
}