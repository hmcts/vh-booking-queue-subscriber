using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Common.Security;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BookingQueueSubscriber
{
    public class VideoWebTokenHandler : DelegatingHandler
    {
        private readonly IMemoryCache _memoryCache;
        private readonly SecurityTokenHandler _tokenHandler;

        private readonly string Secret;
        private const string TokenCacheKey = "VideoWebToken";

        public VideoWebTokenHandler(
            // JwtSecurityTokenHandler tokenHandler,
            IOptions<ServicesConfiguration> servicesConfiguration,
            IMemoryCache memoryCache
           )
        {
            Secret = servicesConfiguration.Value.InternalEventSecret; 
            _memoryCache = memoryCache;
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = _memoryCache.Get<string>(TokenCacheKey);
            if (string.IsNullOrEmpty(token))
            {
                int expireMinutes = 20;
                var symmetricKey = Convert.FromBase64String(Secret);

                var now = DateTime.UtcNow;
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(symmetricKey),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var securityToken = _tokenHandler.CreateToken(tokenDescriptor);
                token = _tokenHandler.WriteToken(securityToken);
                var tokenExpireDateTime = securityToken.ValidTo.AddMinutes(-1);
                _memoryCache.Set(TokenCacheKey, token, tokenExpireDateTime);
            }

            request.Headers.Add("Authorization", $"Bearer {token}");
            return await base.SendAsync(request, cancellationToken);
        }

        
    }
}