using System;
using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using System.Linq;
using ZohoIntegration.TimeLogs.Models;
using System.Data;

namespace ZohoIntegration.TimeLogs.Repositories;
public class TimeLogRelation
{
    internal readonly StorageAccountTableConnection _tableConnection;
    internal readonly TableClient tableClient;

    public TimeLogRelation(StorageAccountTableConnection tableConnection)
    {
        _tableConnection = tableConnection;
        tableClient = tableConnection.GetTableClient("TimeLogRelation");
    }

    public TimeLogRelationEntity SaveRelation(string brId, string ukId)
    {
        var entity = new TimeLogRelationEntity() {
            BRTimeLogID = brId,
            UKTimeLogID = ukId,
            PartitionKey = "1",
            RowKey = Guid.NewGuid().ToString(),
            ETag = new ETag(),
        };

        tableClient.AddEntity(entity);

        return entity;
    }

    public void DeleteRelation (string brId, string ukId) 
    {
        var entity = GetByIds(brId, ukId);

        if(entity is not null)
        {
            tableClient.DeleteEntity(entity.PartitionKey, entity.RowKey);
            return;
        }

        throw new InvalidOperationException($"the given IDs: 'brId = {brId}, ukId = {ukId}' have no relationship");
    }

    public TimeLogRelationEntity GetByIds(string brId, string ukId) 
    {
        var result = tableClient.Query<TimeLogRelationEntity>(filter: table => table.UKTimeLogID == ukId && table.BRTimeLogID == brId);

        return result.FirstOrDefault() ?? throw new InvalidOperationException($"The combination of ids (BR:{brId} - UK:{ukId}) does not match");
    }

    public TimeLogRelationEntity GetByUKTimelogID(string id)
    {
        var result = tableClient.Query<TimeLogRelationEntity>(filter: table => table.UKTimeLogID == id);

        return result.FirstOrDefault() ?? throw new InvalidOperationException($"No Relation found for the given timelog ID {id}");
    }

    public TimeLogRelationEntity GetByBRTimelogID(string id)
    {
        var result = tableClient.Query<TimeLogRelationEntity>(filter: table => table.BRTimeLogID == id);

        return result.FirstOrDefault() ?? throw new InvalidOperationException($"No Relation found for the given timelog ID {id}");
    }

}
