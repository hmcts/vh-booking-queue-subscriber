// Global using directives

global using System;
global using System.Collections.Generic;
global using System.Diagnostics.CodeAnalysis;
global using System.IdentityModel.Tokens.Jwt;
global using System.IO;
global using System.Linq;
global using System.Net.Http;
global using System.Reflection;
global using System.Threading;
global using System.Threading.Tasks;
global using BookingQueueSubscriber.Common.Configuration;
global using BookingQueueSubscriber.Common.Logging;
global using BookingQueueSubscriber.Common.Security;
global using BookingQueueSubscriber.Services;
global using BookingQueueSubscriber.Services.MessageHandlers;
global using BookingQueueSubscriber.Services.MessageHandlers.Core;
global using Microsoft.ApplicationInsights.Extensibility;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;