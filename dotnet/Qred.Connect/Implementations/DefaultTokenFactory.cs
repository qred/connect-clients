using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qred.Connect.Abstractions;

namespace Qred.Connect.Implementations
{
  ///
  public class DefaultTokenFactory : ITokenFactory
  {
    private readonly ConnectConfig options;
    private readonly LazyExpiryAsync<TokenResponse> lazyToken;
    private readonly ILogger<DefaultTokenFactory> logger;
    ///
    public DefaultTokenFactory(IOptions<ConnectConfig> options, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
    {
      logger = loggerFactory.CreateLogger<DefaultTokenFactory>();
      this.options = options.Value ?? throw new ArgumentNullException(nameof(options));
      lazyToken = new LazyExpiryAsync<TokenResponse>(async () =>
        {
          // discover endpoints from metadata
          var disco = await new DiscoveryClient(this.options.Identity)
          {
            Policy =
                {
                    RequireHttps = false,
                    //ValidateEndpoints = false,
                    //ValidateIssuerName = false
                }
          }.GetAsync();
          if (disco.IsError)
          {
            logger.LogError("Couldn't use discovery endpoint {error} {errorType} {exception}",
                    disco.Error, disco.ErrorType, disco.Exception);
            throw new Exception("Couldn't use discovery endpoint", disco.Exception);
          }
          var tokenClient = new TokenClient(disco.TokenEndpoint, this.options.PartnerId, this.options.Secret,
              style: AuthenticationStyle.BasicAuthentication);
          var tokenResponse = await tokenClient.RequestClientCredentialsAsync(this.options.Scopes);
          if (tokenResponse.IsError)
          {
            logger.LogError("Couldn't get token response {error} {errorType} {exception}",
                    tokenResponse.Error, tokenResponse.ErrorType, tokenResponse.Exception);
            throw new Exception("Couldn't get token response", tokenResponse.Exception);
          }
          return Tuple.Create(tokenResponse, DateTime.Now.AddSeconds(tokenResponse.ExpiresIn));
        });
    }
    ///
    public async Task<TokenResponse> GetToken() => await lazyToken.GetValue();
  }
}