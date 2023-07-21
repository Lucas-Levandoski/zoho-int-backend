using System;
using System.Data;
using System.Linq;
using Azure;
using Azure.Data.Tables;
using ZohoIntegration.TimeLogs.Enums;
using ZohoIntegration.TimeLogs.Models;
using ZohoIntegration.TimeLogs.Repositories;

namespace ZohoIntegration.TimeLogs;
public class AccessTokenRepo
{
    internal readonly StorageAccountTableConnection _tableConnection;
    internal readonly TableClient tableClient;
    internal readonly string rowKey = "ACCESS_TOKEN";

    public AccessTokenRepo(StorageAccountTableConnection tableConnection)
    {
        _tableConnection = tableConnection;
        tableClient = tableConnection.GetTableClient("AccessToken");
    }

    public string? GetCurrentToken(TargetZohoAccount target)
    {
        var result = tableClient.Query<AccessTokenEntity>(filter: table => table.RowKey == rowKey && table.PartitionKey == target.ToString()).FirstOrDefault();

        return result?.CurrentAccessToken;
    }

    public void SaveAccessToken(string newToken, TargetZohoAccount target)
    {
        tableClient.DeleteEntity(target.ToString(), rowKey);

        var entity = new AccessTokenEntity()
        {
            CurrentAccessToken = newToken,
            PartitionKey = target.ToString(),
            RowKey = rowKey,
            ETag = new ETag(),
        };

        tableClient.AddEntity(entity);
    }
}
