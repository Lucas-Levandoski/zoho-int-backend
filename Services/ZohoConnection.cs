using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ZohoIntegration.TimeLogs.Enums;
using ZohoIntegration.TimeLogs.Models;

namespace ZohoIntegration.TimeLogs.Services;
public class ZohoConnection
{
    private string ukRefreshToken = "";
    private string brRefreshToken = "";
    private readonly AccessTokenRepo _accessTokenRepo;
    private HttpClient zUKPeopleHttpClient;
    private HttpClient zBRPeopleHttpClient;
    private HttpClient zAccountsHttpClient;

    public ZohoConnection(AccessTokenRepo accessTokenRepo)
    {
        _accessTokenRepo = accessTokenRepo;

        ukRefreshToken = Environment.GetEnvironmentVariable("ZOHO_UK_REFRESH_TOKEN") ?? throw new SystemException("Missing ZOHO_UK_REFRESH_TOKEN env var");
        brRefreshToken = Environment.GetEnvironmentVariable("ZOHO_BR_REFRESH_TOKEN") ?? throw new SystemException("Missing ZOHO_BR_REFRESH_TOKEN env var");

        zUKPeopleHttpClient = new () 
        {
            BaseAddress = new Uri(Environment.GetEnvironmentVariable("ZOHO_PEOPLE_URI") ?? throw new SystemException("Missing ZOHO_PEOPLE_URI env var"))
        };

        zBRPeopleHttpClient = new () 
        {
            BaseAddress = new Uri(Environment.GetEnvironmentVariable("ZOHO_PEOPLE_URI") ?? throw new SystemException("Missing ZOHO_PEOPLE_URI env var"))
        };

        zAccountsHttpClient = new () 
        {
            BaseAddress = new Uri(Environment.GetEnvironmentVariable("ZOHO_ACCOUNTS_URI") ?? throw new SystemException("Missing ZOHO_ACCOUNTS_URI env var"))
        };

        zUKPeopleHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Zoho-oauthtoken", _accessTokenRepo.GetCurrentToken(TargetZohoAccount.UK));
        zBRPeopleHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Zoho-oauthtoken", _accessTokenRepo.GetCurrentToken(TargetZohoAccount.BR));
    }

    private async Task RefreshAccessToken(TargetZohoAccount target = TargetZohoAccount.UK) 
    {
        string refreshToken = "";
        string clientId = "";
        string clientSecret = "";

        switch(target) {
            case TargetZohoAccount.UK:
                refreshToken = ukRefreshToken;
                clientId = Environment.GetEnvironmentVariable("ZOHO_UK_CLIENT_ID")  ?? throw new SystemException("Missing ZOHO_UK_CLIENT_ID env var");
                clientSecret = Environment.GetEnvironmentVariable("ZOHO_UK_CLIENT_SECRET")  ?? throw new SystemException("Missing ZOHO_UK_CLIENT_SECRET env var");
                break; 
            case TargetZohoAccount.BR:
                refreshToken = brRefreshToken;
                clientId = Environment.GetEnvironmentVariable("ZOHO_BR_CLIENT_ID")  ?? throw new SystemException("Missing ZOHO_BR_CLIENT_ID env var");
                clientSecret = Environment.GetEnvironmentVariable("ZOHO_BR_CLIENT_SECRET")  ?? throw new SystemException("Missing ZOHO_BR_CLIENT_SECRET env var");
                break; 
        }


        Dictionary<string, string> queryParams = new (){
            { "refresh_token", refreshToken },
            { "client_id",  clientId},
            { "client_secret", clientSecret },
            { "grant_type", "refresh_token" }
        };

        var accessTokenRes = await zAccountsHttpClient.PostAsync("", new FormUrlEncodedContent(queryParams));

        RefreshTokenView? result = JsonConvert.DeserializeObject<RefreshTokenView>(await accessTokenRes.Content.ReadAsStringAsync());

        if(result?.accessToken == null)
            throw new DataException("missing access token from query");

        _accessTokenRepo.SaveAccessToken(result.accessToken, target);

        switch(target)
        {
            case TargetZohoAccount.UK:
                zUKPeopleHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Zoho-oauthtoken", result.accessToken);
                break;
            case TargetZohoAccount.BR:
                zBRPeopleHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Zoho-oauthtoken", result.accessToken);
                break;
        }
    }


    public async Task<T> GetAsync<T>(string uri, TargetZohoAccount target = TargetZohoAccount.UK)
    {
        HttpClient httpClient;

        switch(target)
        {
            case TargetZohoAccount.UK:
                httpClient = zUKPeopleHttpClient;
                break;
            case TargetZohoAccount.BR:
                httpClient = zBRPeopleHttpClient;
                break;
            default:
                throw new SystemException("missing target zoho account");
        }
        
        var apiReturn = await httpClient.GetAsync(uri);

        // if the request fails it will try to refresh the access token and try only once more
        if(!apiReturn.IsSuccessStatusCode) 
        {
            await RefreshAccessToken(target);
            apiReturn = await httpClient.GetAsync(uri);
        }

        T? result = JsonConvert.DeserializeObject<T>(await apiReturn.Content.ReadAsStringAsync());

        if(result == null)
            throw new DataException($"no result found for the given URL:{uri}");

        return result;
    }


    public async Task<T> PostAsync<T>(string uri, HttpContent? content = null, TargetZohoAccount target = TargetZohoAccount.UK)
    {
        HttpClient httpClient;

        switch(target)
        {
            case TargetZohoAccount.UK:
                httpClient = zUKPeopleHttpClient;
                break;
            case TargetZohoAccount.BR:
                httpClient = zBRPeopleHttpClient;
                break;
            default:
                throw new SystemException("missing target zoho account");
        }

        var apiReturn = await httpClient.PostAsync(uri, content);

        // if the request fails it will try to refresh the access token and try only once more
        if(!apiReturn.IsSuccessStatusCode) 
        {
            await RefreshAccessToken(target);
            apiReturn = await httpClient.PostAsync(uri, content);
        }

        T? result = JsonConvert.DeserializeObject<T>(await apiReturn.Content.ReadAsStringAsync());

        if(result == null)
            throw new DataException($"no return for the given URL:{uri}");

        return result;
    }


    public async Task PostAsync(string uri, HttpContent? content = null, TargetZohoAccount target = TargetZohoAccount.UK)
    {
        HttpClient httpClient;

        switch(target)
        {
            case TargetZohoAccount.UK:
                httpClient = zUKPeopleHttpClient;
                break;
            case TargetZohoAccount.BR:
                httpClient = zBRPeopleHttpClient;
                break;
            default:
                throw new SystemException("missing target zoho account");
        }

        
        var apiReturn = await httpClient.PostAsync(uri, content);

        // if the request fails it will try to refresh the access token and try only once more
        if(!apiReturn.IsSuccessStatusCode) 
        {
            await RefreshAccessToken(target);
            apiReturn = await httpClient.PostAsync(uri, content);

            if (!apiReturn.IsSuccessStatusCode)
                throw new DataException($"The post method for the given URL -- {uri} -- didn't work even after refreshing access token");
        }
    }
}
