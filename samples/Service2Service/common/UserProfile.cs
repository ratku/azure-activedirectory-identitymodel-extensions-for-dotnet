//------------------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.IdentityModel.Logging;
using Newtonsoft.Json;

namespace S2SCommon
{
    /// <summary>
    /// Contains OpenIdConnect configuration that can be populated from a json string.
    /// </summary>
    [JsonObject]
    public class UserProfile
    {
        /// <summary>
        /// Deserializes the json string into an <see cref="OpenIdConnectConfiguration"/> object.
        /// </summary>
        /// <param name="json">json string representing the configuration.</param>
        /// <returns><see cref="OpenIdConnectConfiguration"/> object representing the configuration.</returns>
        /// <exception cref="ArgumentNullException">If 'json' is null or empty.</exception>
        /// <exception cref="ArgumentException">If 'json' fails to deserialize.</exception>
        public static UserProfile Create(string json)
        {
            if (string.IsNullOrEmpty(json))
                throw LogHelper.LogArgumentNullException(nameof(json));

            return new UserProfile(json);
        }

        /// <summary>
        /// Serializes the <see cref="OpenIdConnectConfiguration"/> object to a json string.
        /// </summary>
        /// <param name="configuration"><see cref="OpenIdConnectConfiguration"/> object to serialize.</param>
        /// <returns>json string representing the configuration object.</returns>
        /// <exception cref="ArgumentNullException">If 'configuration' is null.</exception>
        public static string Write(UserProfile configuration)
        {
            if (configuration == null)
                throw LogHelper.LogArgumentNullException(nameof(configuration));

            return JsonConvert.SerializeObject(configuration);
        }

        /// <summary>
        /// Initializes an new instance of <see cref="OpenIdConnectConfiguration"/>.
        /// </summary>
        public UserProfile()
        {
        }

        /// <summary>
        /// Initializes an new instance of <see cref="OpenIdConnectConfiguration"/> from a json string.
        /// </summary>
        /// <param name="json">a json string containing the metadata</param>
        /// <exception cref="ArgumentNullException">If 'json' is null or empty.</exception>
        public UserProfile(string json)
        {
            if(string.IsNullOrEmpty(json))
                throw LogHelper.LogArgumentNullException(nameof(json));

            JsonConvert.PopulateObject(json, this);
        }

        /// <summary>
        /// When deserializing from JSON any properties that are not defined will be placed here.
        /// </summary>
        [JsonExtensionData]
        public virtual IDictionary<string, object> AdditionalData { get; } = new Dictionary<string, object>();

        //"odata.metadata":"https://graph.windows.net/add29489-7269-41f4-8841-b63c95564420/$metadata#directoryObjects/Microsoft.WindowsAzure.ActiveDirectory.User/@Element",
        //"odata.type":"Microsoft.WindowsAzure.ActiveDirectory.User",
        //"assignedLicenses":[],
        //"assignedPlans":[],
        //"country":null,
        //"department":null,
        //"dirSyncEnabled":null,
        //"facsimileTelephoneNumber":null,
        //"givenName":"User",
        //"immutableId":null,
        //"jobTitle":null,
        //"lastDirSyncTime":null,
        //"mail":null,
        //"mailNickname":"User1",
        //"mobile":null,
        //"otherMails":[],
        //"passwordPolicies":"None",
        //"passwordProfile":null,
        //"physicalDeliveryOfficeName":null,
        //"postalCode":null,
        //"preferredLanguage":null,
        //"provisionedPlans":[],
        //"provisioningErrors":[],
        //"proxyAddresses":[],
        //"state":null,
        //"streetAddress":null,
        //"surname":"1",
        //"telephoneNumber":null,
        //"usageLocation":null,
        //"userPrincipalName":"User1@Cyrano.onmicrosoft.com",
        //"userType":"Member"

        /// <summary>
        /// Gets the collection of 'acr_values_supported'
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "objectId", Required = Required.Default)]
        public string ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the accountEnabled.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "accountEnabled", Required = Required.Default)]
        public bool AccountEnabled { get; set; }

        /// <summary>
        /// Gets or sets the check_session_iframe.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "objectType", Required = Required.Default)]
        public string ObjectType { get; set; }

        /// <summary>
        /// Gets or sets the end session endpoint.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "city", Required = Required.Default)]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the end session endpoint.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "userPrincipalName", Required = Required.Default)]
        public string UserPrincipalName { get; set; }

        /// <summary>
        /// Gets or sets the end session endpoint.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "userType", Required = Required.Default)]
        public string UserType { get; set; }

        /// <summary>
        /// Gets or sets the 'issuer'.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "displayName", Required = Required.Default)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets the collection of 'id_token_signing_alg_values_supported'.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "provisionedPlans", Required = Required.Default)]
        public ICollection<string> ProvisionedPlans { get; } = new Collection<string>();

    }
}
