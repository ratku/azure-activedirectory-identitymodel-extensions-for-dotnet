using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using System.Web.Http;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.S2S.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace S2SBackend.Controllers
{
    public class S2STokenProtectedController : ApiController
    {
        private const string _audience = "http://localhost:48274/";
        private const string _authority = "https://testingsts.azurewebsites.net/";
        private const string _clientId = "api-002";
        private ConfigurationManager<OpenIdConnectConfiguration> _configManager;

        private const string _middleTierClientId = "api-001";


        public S2STokenProtectedController()
        {
            // Step 2.1 - reach out for configuration (this can be cached).
            _configManager = new ConfigurationManager<OpenIdConnectConfiguration>(_authority + ".well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
        }

        [HttpGet]
        public IEnumerable<string> S2SProtectedCall()
        {
            try
            {
                // Step 1 - ensure POP header is well formed
                var result = TokenValidator.ValidateHttpAuthenticator(HttpContext.Current.Request.Headers, HttpContext.Current.Request.Url);

                // Step 2 - validate Tokens in the header
                var config = _configManager.GetConfigurationAsync(CancellationToken.None).Result;

                // Step 2.1 - validate App Token
                var validatedAppToken = TokenValidator.ValidateToken(result.AppToken, config, _audience).SecurityToken as JwtSecurityToken;

                // Step 2.1 - validate Payload Token, Audience is unknown as token is forwarded
                var validatePayloadToken = TokenValidator.ValidateToken(result.PayloadToken, config).SecurityToken as JwtSecurityToken;

                // Step 3 - check for claims in App / Payload tokens
                // application specific code

                return new string[]
                {
                    $"App Token Validated: '{validatedAppToken.Claims}'",
                    $"Payload Token Validated: '{validatePayloadToken.Claims}'",
                };

            }
            catch (Exception ex)
            {
                return new string[]
                {
                    $"Site: '{ WebApiApplication.SiteName}'",
                    $"AuthManager.Validate threw: '{ex}'"
                };
            }
        }
    }
}
