﻿using System;
using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Common.Security;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests
{
    public class AzureTokenProviderTests
    {
        [Test]
        public void should_get_access_token()
        {
            var azureAdConfigurationOptions = Options.Create(new AzureAdConfiguration { TenantId = "teanantid" });
            var azureTokenProvider = new AzureTokenProvider(azureAdConfigurationOptions);
            Assert.ThrowsAsync<System.UnauthorizedAccessException>(async () => await azureTokenProvider.GetClientAccessToken("1234", "1234", "1234"));
        }
    }
}