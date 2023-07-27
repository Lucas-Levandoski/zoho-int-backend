using System;
using Azure;
using Azure.Data.Tables;
using System.Linq;
using ZohoIntegration.TimeLogs.Models;
using ZohoIntegration.TimeLogs.Repositories;
using System.Collections.Generic;

namespace ZohoIntegration.TimeLogs.Repositories;
public class JobNameRelation
{
    internal readonly StorageAccountTableConnection _tableConnection;
    internal readonly TableClient tableClient;

    public JobNameRelation(StorageAccountTableConnection tableConnection)
    {
        _tableConnection = tableConnection;
        tableClient = tableConnection.GetTableClient("JobNameRelation");
    }

    public JobNameRelationEntity SaveRelation(
        JobNameRelationInput relationInput,
        string? rowKey = null)
    {
        var entity = new JobNameRelationEntity()
        {
            BRJobName = relationInput.brJobName,
            BRJobId = relationInput.brJobId,
            BRJobProjectId = relationInput.brJobProjectId,
            BRJobProjectName = relationInput.brJobProjectName,
            UKJobName = relationInput.ukJobName,
            UKJobId = relationInput.ukJobId,
            UKJobProjectId = relationInput.ukJobProjectId,
            UKJobProjectName = relationInput.ukJobProjectName,
            PartitionKey = "2",
            RowKey = rowKey ?? Guid.NewGuid().ToString(),
            ETag = new ETag(),
        };

        tableClient.AddEntity(entity);

        return entity;
    }

    
    public JobNameRelationEntity SaveRelation(
        JobNameRelationEntity relationInput,
        string? rowKey = null)
    {
        var entity = new JobNameRelationEntity()
        {
            BRJobName = relationInput.BRJobName,
            BRJobId = relationInput.BRJobId,
            BRJobProjectId = relationInput.BRJobProjectId,
            BRJobProjectName = relationInput.BRJobProjectName,
            UKJobName = relationInput.UKJobName,
            UKJobId = relationInput.UKJobId,
            UKJobProjectId = relationInput.UKJobProjectId,
            UKJobProjectName = relationInput.UKJobProjectName,
            PartitionKey = "2",
            RowKey = rowKey ?? Guid.NewGuid().ToString(),
            ETag = new ETag(),
        };

        tableClient.AddEntity(entity);

        return entity;
    }

    public void DeleteRelation(string partitionKey, string rowKey)
    {
        tableClient.DeleteEntity(partitionKey, rowKey);
    }

    public JobNameRelationEntity? getByBRId(string brId)
    {
        var result = tableClient.Query<JobNameRelationEntity>(filter: table => table.BRJobId == brId).FirstOrDefault();

        return result;
    }

    public JobNameRelationEntity? getByRowKey(string rowKey)
    {
        var result = tableClient.Query<JobNameRelationEntity>(filter: table => table.RowKey == rowKey).FirstOrDefault();

        return result;
    }

    public (bool isExists, string rowId) CheckExistingRelation(string BRJobId, string UKJobId)
    {
        var result = tableClient.Query<JobNameRelationEntity>(filter: table => table.BRJobId == BRJobId && table.UKJobId == UKJobId).FirstOrDefault();

        return (result != null, result?.RowKey ?? "");
    }

    public bool CheckExistingRelationByRowKey(string RowKey)
    {
        var result = tableClient.Query<JobNameRelationEntity>(filter: table => table.RowKey == RowKey).FirstOrDefault();

        return result?.RowKey != null;
    }

    public (bool isExists, JobNameRelationEntity? relation) CheckExistingRelationByBRId(string BRJobId)
    {
        var result = tableClient.Query<JobNameRelationEntity>(filter: table => table.BRJobId == BRJobId).FirstOrDefault();

        return (result?.RowKey != null, result);
    }

    public List<JobNameRelationEntity> ListAll()
    {
        var result = tableClient.Query<JobNameRelationEntity>().ToList();

        return result;
    }

    public List<JobNameRelationEntity> ListAllJobsByBRId(string jobId) 
    {
        var result = tableClient.Query<JobNameRelationEntity>(filter: table => table.BRJobId == jobId).ToList();

        return result;
    }

    public List<JobNameRelationEntity> ListAllJobsByUKId(string jobId) 
    {
        var result = tableClient.Query<JobNameRelationEntity>(filter: table => table.UKJobId == jobId).ToList();

        return result;
    }

    public void UpdateOrCreateList(List<JobNameRelationEntity> jobs)
    {
        foreach(var job in jobs)
        {
            if (job.PartitionKey != null && job.PartitionKey != default && 
                job.RowKey != null && job.RowKey != default)
            {
                tableClient.DeleteEntity(job.PartitionKey, job.RowKey);

                tableClient.AddEntity(job);

                break;
            }

            SaveRelation(job);
        }
    }
}
