using System;
using System.Linq;
using Azure;
using Azure.Data.Tables;
using ZohoIntegration.TimeLogs.Models;
using ZohoIntegration.TimeLogs.Repositories;

namespace ZohoIntegration.TimeLogs.Repositories;
public class ProjectNameRelation
{
    internal readonly StorageAccountTableConnection _tableConnection;
    internal readonly TableClient tableClient;

    public ProjectNameRelation(StorageAccountTableConnection tableConnection)
    {
        _tableConnection = tableConnection;
        tableClient = tableConnection.GetTableClient("ProjectNameRelation");
    }

    public ProjectNameRelationEntity SaveRelation(string BRName, string BRID, string UKName, string UKID, string? rowKey = null)
    {
        var entity = new ProjectNameRelationEntity()
        {
            BRProjectName = BRName,
            BRProjectID = BRID,
            UKProjectName = UKName,
            UKProjectID = UKID,
            PartitionKey = "2",
            RowKey = rowKey ?? Guid.NewGuid().ToString(),
            ETag = new ETag(),
        };

        tableClient.AddEntity<ProjectNameRelationEntity>(entity);

        return entity;
    }

    public void DeleteRelation(string partitionKey, string rowKey)
    {
        tableClient.DeleteEntity(partitionKey, rowKey);
    }

    public (bool isExists, string rowId) CheckExistingRelation(string BRName, string UKName)
    {
        var result = tableClient.Query<ProjectNameRelationEntity>(filter: table => table.BRProjectName == BRName && table.UKProjectName == UKName).FirstOrDefault();

        return (result != null, result?.RowKey ?? "");
    }

    public (bool isExists, string oldBRName, string oldUKName, string partitionKey) CheckExistingRelation(string RowKey)
    {
        var result = tableClient.Query<ProjectNameRelationEntity>(filter: table => table.RowKey == RowKey).FirstOrDefault();

        return (result?.RowKey != null, result?.BRProjectName ?? "", result?.UKProjectName ?? "", result?.PartitionKey ?? "");
    }
}
