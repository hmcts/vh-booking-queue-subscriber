using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BookingQueueSubscriber.Common.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BookingQueueSubscriber
{
    public class VideoWebTokenHandler : DelegatingHandler
    {
        private readonly IMemoryCache memoryCache;
        private readonly SecurityTokenHandler tokenHandler;

        private readonly string Secret;
        private const string TokenCacheKey = "VideoWebToken";

        public VideoWebTokenHandler(
            IOptions<ServicesConfiguration> _servicesConfiguration,
            IMemoryCache _memoryCache
           )
        {
            Secret = _servicesConfiguration.Value.InternalEventSecret; 
            this.memoryCache = _memoryCache;
            tokenHandler = new JwtSecurityTokenHandler();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = memoryCache.Get<string>(TokenCacheKey);
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

                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                token = tokenHandler.WriteToken(securityToken);
                var tokenExpireDateTime = securityToken.ValidTo.AddMinutes(-1);
                memoryCache.Set(TokenCacheKey, token, tokenExpireDateTime);
            }

            request.Headers.Add("Authorization", $"Bearer {token}");
            return await base.SendAsync(request, cancellationToken);
        }

        
    }
}