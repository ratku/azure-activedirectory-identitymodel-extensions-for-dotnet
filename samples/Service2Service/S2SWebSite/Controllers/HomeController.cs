//-------------------------------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.S2S.Tokens;
using Microsoft.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;

namespace S2SWebSite.Controllers
{
    public class HomeController : Controller
    {
        private ConfigurationManager<OpenIdConnectConfiguration> _configManager = new ConfigurationManager<OpenIdConnectConfiguration>(Authority + ".well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
        private OpenIdConnectProtocolValidator _protocolValidator = new OpenIdConnectProtocolValidator();
        private JsonWebTokenHandler _tokenHandler = new JsonWebTokenHandler();

        // public constants related to this site: S2SWebSite
        public const string Address = "http://localhost:48272/";
        public const string Authority = "https://testingsts.azurewebsites.net/";
        public const string ClientId = "client-001";
        public const string RedirectUri = Address;
        public const string Tennant = "add29489-7269-41f4-8841-b63c95564420";
        public const string Thumbprint = "5C346E0642C1113812C7775F0CB5336D8DFAFC4B";

        // S2SMiddleTier metadata
        public const string MiddleTierAddress = "http://localhost:48273/";
        public const string MiddleTierClientId = "api-001";
        public const string MiddleTierEndpoint = MiddleTierAddress + "api/AccessTokenProtected/ProtectedApi";

        public ActionResult Index()
        {
            var signingCredentials = KeyingMaterial.JsonWebKeyRsa256SigningCredentials;

            var payload = new JObject()
            {
                { JwtRegisteredClaimNames.Email, "Bob@contoso.com"},
                { JwtRegisteredClaimNames.GivenName, "Bob"},
                { JwtRegisteredClaimNames.Iss, "http://Default.Issuer.com" },
                { JwtRegisteredClaimNames.Aud, "http://Default.Audience.com" },
                { JwtRegisteredClaimNames.Nbf, "2017-03-18T18:33:37.080Z" },
                { JwtRegisteredClaimNames.Exp, "2021-03-17T18:33:37.080Z" }
            };

            var accessToken = _tokenHandler.CreateJWSAsync(payload, signingCredentials).Result;

            ViewBag.ClientId = ClientId;
            ViewBag.Error = string.Empty;
            ViewBag.Response = "Signin using AzureAD.";
            ViewBag.Title = "S2SWebSite";

            if (Request.IsAuthenticated)
            {
                try
                {
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add(AuthenticationConstants.AuthorizationHeader, AuthenticationConstants.BearerWithSpace + accessToken);
                    var httpResponse = httpClient.GetAsync(MiddleTierEndpoint).Result;

                    ViewBag.Response = httpResponse.Content.ReadAsStringAsync().Result;
                }
               catch (Exception ex)
               {
                   ViewBag.Error = ex.ToString();
               }

               ViewBag.WebAppClientId = ClientId;
               ViewBag.Identity = HttpContext.User.Identity;
            }

            ViewData["Name"] = "S2SWebSite";

            return View();
        }
    }
}
