using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.S2S;
using Microsoft.IdentityModel.S2S.Jwt;
using Microsoft.IdentityModel.S2S.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

using AuthenticationContext = Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext;

namespace S2SWebSite
{
    public class Startup
    {
        public static S2SAuthenticationManager AuthManager;

        // public constants related to this site: S2SWebSite
        public const string Address = "http://localhost:38272/";
        public const string AuthType = "OpenIdConnect";
        public const string Authority = "https://login.microsoftonline.com/cyrano.onmicrosoft.com/";
        public const string ClientId = "905a5e2a-ebf5-4b70-8eb0-fd26303b6a5f";
        public const string ClientSecret = "khdgqYyjZzRH1fASLriMF/N249aiSpTCxEetnUzmwNA=";
        public const string RedirectUri = Address;
        public const string LogDir = @"C:\Logs\SAL";
        public const string SiteName = "S2SWebSite";
        public const string Thumbprint = "bc497e2436646bfe25f32cb2e1e62700f8985f84";

        // Outbound policy names
        public const string AppAssertedUserV1Policy = "AppAssertedUserV1Policy";
        public const string AppTokenPolicy = "AppTokenPolicy";
        public const string AccessTokenPolicy = "AccessTokenPolicy";

        // S2SMiddleTier metadata
        public const string MiddleTierAddress = "http://localhost:38273/";
        public const string MiddleTierClientId = "2d149917-123d-4ba3-8774-327b875f5540";//"260b50e2-c38b-4aeb-9217-347dc2caefb3";
        public const string MiddleTierEndpoint = MiddleTierAddress + "api/AccessTokenProtected/ProtectedApi";

        private static Dictionary<string, AuthenticationResult> _accessTokens = new Dictionary<string, AuthenticationResult>();

        public void Configuration()
        {
            SetupSAL();
            var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(Authority + ".well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
            var config = configManager.GetConfigurationAsync(CancellationToken.None).Result;
            var tokenEndpoint = config.TokenEndpoint;
            var openIdMessage = new OpenIdConnectMessage();
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
            requestMessage.Content = new FormUrlEncodedContent(openIdMessage.Parameters);
            HttpClient httpClient = new HttpClient();
            var responseMessage = httpClient.SendAsync(requestMessage).Result;
            responseMessage.EnsureSuccessStatusCode();
            var tokenResonse = responseMessage.Content.ReadAsStringAsync().Result;
            var jsonTokenResponse = JObject.Parse(tokenResonse);
        }

        private void SetupLogging()
        {
            if (!Directory.Exists(LogDir))
                Directory.CreateDirectory(LogDir);

            TextWriterEventListener listener = new TextWriterEventListener(Path.Combine(LogDir, SiteName + ".txt"));
            S2SEventSource.Instance.EventLevel = EventLevel.Verbose;
            listener.EnableEvents(S2SEventSource.Instance, EventLevel.Verbose);
            S2SEventSource.Instance.WriteInformation("S2SWebSite, starting up: " + DateTime.UtcNow.ToString());
        }

        private void SetupSAL()
        {
            SetupLogging();

            var jwtHandler = new JwtAuthenticationHandler();
            jwtHandler.AddOutboundPolicy(
                AccessTokenPolicy,
                new JwtOutboundPolicy
                {
                    AuthenticationMode = S2SAuthenticationMode.AccessToken,
                    Authority = Authority,
                    ClientId = ClientId,
                    Certificate = FindCertificate(StoreName.My, StoreLocation.LocalMachine, Thumbprint),
                    Resource = MiddleTierClientId,

                });

            jwtHandler.AddOutboundPolicy(
                AppAssertedUserV1Policy,
                new JwtOutboundPolicy
                {
                    AuthenticationMode = S2SAuthenticationMode.AppAssertedUserV1,
                    Authority = Authority,
                    ClientId = ClientId,
                    Certificate = FindCertificate(StoreName.My, StoreLocation.LocalMachine, Thumbprint),
                    Resource = MiddleTierClientId,

                });

            jwtHandler.AddOutboundPolicy(
                AppTokenPolicy,
                new JwtOutboundPolicy
                {
                    AuthenticationMode = S2SAuthenticationMode.AppToken,
                    Authority = Authority,
                    ClientId = ClientId,
                    Certificate = FindCertificate(StoreName.My, StoreLocation.LocalMachine, Thumbprint),
                    Resource = MiddleTierClientId,

                });

            AuthManager = new S2SAuthenticationManager(jwtHandler);
        }

        public static bool TryGetAccessToken(string authority, string clientId, string resource, out string accessToken)
        {
            AuthenticationResult authenticationResult = null;
            if (_accessTokens.TryGetValue(authority + clientId + resource, out authenticationResult))
            {
                accessToken = authenticationResult.AccessToken;
                return true;
            }

            accessToken = null;
            return false;
        }

        private static X509Certificate2 FindCertificate(StoreName storeName, StoreLocation storeLocation, string thumbprint)
        {
            X509Store x509Store = new X509Store(storeName, storeLocation);
            x509Store.Open(OpenFlags.ReadOnly);
            try
            {
                foreach (var cert in x509Store.Certificates)
                {
                    if (cert.Thumbprint.Equals(thumbprint, StringComparison.OrdinalIgnoreCase))
                    {
                        return cert;
                    }
                }

                throw new ArgumentException(
                    string.Format("S2SWebsite communicates with AzureAD using a Certificate with thumbprint: '{0}'. SAL_SDK includes '<ROOT>\\src\\Certs\\S2SWebSite.pfx' that needs to be imported into 'LocalComputer\\Personal' (password is: S2SWebSite).{1}'<ROOT>\\src\\ToolsAndScripts\\AddPfxToCertStore.ps1' can be used install certs.{1}Make sure to open the powershell window as an administrator.", thumbprint, Environment.NewLine));
            }
            finally
            {
                if (x509Store != null)
                {
                    x509Store.Close();
                }
            }
        }
    }
}
