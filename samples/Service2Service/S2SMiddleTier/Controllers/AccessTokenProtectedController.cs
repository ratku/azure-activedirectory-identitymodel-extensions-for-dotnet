using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.S2S.Tokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens.Jwt.Tests;

namespace S2SMiddleTier.Controllers
{
    public class AccessTokenProtectedController : ApiController
    {
        private const string _authority = "https://testingsts.azurewebsites.net/";
        private const string _clientId = "client-001";
        private const string _backend = "http://localhost:48274/";
        private const string _thumbprint = "8BDD5C76F165FA88C5A73E978D0522C47F934C90";
        private const string _webApiCall = _backend + "api/S2STokenProtected/S2SProtectedCall";
        private const string _webApiCall2 = _backend + "api/S2STokenProtected/S2SProtectedCall2";
        private JsonWebTokenHandler _jsonWebTokenHandler = new JsonWebTokenHandler();

        [HttpGet]
        public async Task<IEnumerable<string>> ProtectedApi()
        {
            var authorizationHeader = HttpContext.Current.Request.Headers.Get(AuthenticationConstants.AuthorizationHeader);
            string token = null;
            try
            {
                if (authorizationHeader.StartsWith(AuthenticationConstants.BearerWithSpace, StringComparison.OrdinalIgnoreCase))
                    token = authorizationHeader.Substring(AuthenticationConstants.BearerWithSpace.Length).Trim();
                else
                    throw new InvalidOperationException();
            }
            catch (Exception ex)
            {
                return new string[]
                {
                    "Site: S2SMiddleTier",
                    $"This exception was thrown during validation: '{ex}'"
                };
            }

            return await ValidateTokenAsync(token);
        }

        /// <summary>
        /// Calls the backend with POP Authenticator
        /// </summary>
        /// <param name="serviceAddress"></param>
        /// <param name="payloadToken"></param>
        /// <returns></returns>
        async Task<IEnumerable<string>> ValidateTokenAsync(string payloadToken)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters()
                {
                    ValidAudience = "http://Default.Audience.com",
                    ValidIssuer = "http://Default.Issuer.com",
                    IssuerSigningKey = KeyingMaterial.JsonWebKeyRsa256SigningCredentials.Key,
                    CryptoProviderFactory = new CryptoProviderFactory()
                    {
                        CustomCryptoProvider = new AsyncCryptoProvider(KeyingMaterial.JsonWebKeyRsa256SigningCredentials.Key, KeyingMaterial.JsonWebKeyRsa256SigningCredentials.Algorithm, false)
                    }
                };
                var tokenValidationResult = await _jsonWebTokenHandler.ValidateJWSAsync(payloadToken, tokenValidationParameters).ConfigureAwait(false);
                var jsonWebToken = tokenValidationResult.SecurityToken as JsonWebToken;
                var email = jsonWebToken.Payload.Value<string>(JwtRegisteredClaimNames.Email);

                if (!email.Equals("Bob@contoso.com"))
                    throw new SecurityTokenException("Token does not contain the correct value for the 'email' claim.");

                return new string[] { "Token was validated." };
            }
            catch (Exception ex)
            {
                return new string[]
                {
                    $"Site: 'S2SMiddleTier threw: '{ex}'"
                };
            }
        }
    }
}
