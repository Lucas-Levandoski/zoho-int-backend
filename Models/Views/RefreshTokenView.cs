using System;
using Newtonsoft.Json;

namespace ZohoIntegration.TimeLogs;
public class RefreshTokenView
{
    [JsonProperty(PropertyName = "access_token")]
    public string accessToken { get; set; }


    [JsonProperty(PropertyName = "api_domain")]
    public string apiDomain { get; set; }

    [JsonProperty(PropertyName = "token_type")]
    public string tokenType { get; set; }

    [JsonProperty(PropertyName = "expires_in")]
    public int expiresIn { get; set; }
}
