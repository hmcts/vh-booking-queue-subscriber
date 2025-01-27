namespace BookingQueueSubscriber.Security
{
    [ExcludeFromCodeCoverage]
    public class VideoWebTokenHandler : DelegatingHandler
    {
        private readonly IMemoryCache _memoryCache;
        private readonly SecurityTokenHandler _tokenHandler;

        private readonly string _secret;
        private const string TokenCacheKey = "VideoWebToken";

        public VideoWebTokenHandler(
            IOptions<ServicesConfiguration> servicesConfiguration,
            IMemoryCache memoryCache
           )
        {
            _secret = servicesConfiguration.Value.InternalEventSecret; 
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
                var symmetricKey = Convert.FromBase64String(_secret);

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