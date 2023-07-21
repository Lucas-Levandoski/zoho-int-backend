using System;
using System.Collections.Generic;
using System.Data;
using ZohoIntegration.TimeLogs.Models;
using ZohoIntegration.TimeLogs.Repositories;

namespace ZohoIntegration.TimeLogs.Services;
public class JobName
{
    private readonly JobNameRelation _jobNameRepo;

    public JobName(JobNameRelation jobNameRelation)
    {
        _jobNameRepo = jobNameRelation;
    }

    public void CreateRelation(JobNameRelationInput relationInput)
    {
        var (isExists, entity) = _jobNameRepo.CheckExistingRelationByBRId(relationInput.brJobId);

        if (isExists)
            throw new InvalidOperationException("This BR Job Id already has a target");

        _jobNameRepo.SaveRelation(relationInput);
    }

    public (string oldBRName, string oldUKName) UpdateRelation(JobNameRelationInput relationInput, string rowKey)
    {
        var keyExists = _jobNameRepo.CheckExistingRelationByRowKey(rowKey);

        if (!keyExists)
        {
            throw new InvalidOperationException($"Relation ID {rowKey} not found");
        }
        
        var (isExists, entity) = _jobNameRepo.CheckExistingRelationByBRId(relationInput.brJobId);

        if (isExists && entity!.RowKey != rowKey)
            throw new InvalidOperationException("This BR Job Id already has a target");

        WriteLine("Changing names from BRNames: {0} -> {1} and UKNames: {2} -> {3}", entity.BRJobName, relationInput.brJobName, entity.UKJobName, relationInput.ukJobName);

        _jobNameRepo.DeleteRelation(entity.PartitionKey, entity.RowKey);

        _jobNameRepo.SaveRelation(relationInput, rowKey);

        return (entity.BRJobName, entity.UKJobName);
    }

    public (string currentBRName, string currentUKName) DeleteRelation(string rowKey)
    {
        var keyExists = _jobNameRepo.CheckExistingRelationByRowKey(rowKey);

        if (!keyExists)
            throw new InvalidOperationException($"Row Key {rowKey} not found");


        var entity = _jobNameRepo.getByRowKey(rowKey) ?? throw new InvalidOperationException($"Row Key {rowKey} not found");;

        _jobNameRepo.DeleteRelation(entity.PartitionKey, entity.RowKey);

        return (entity.BRJobName, entity.UKJobName);
    }

    public JobNamesRelationOutput ListAll() 
    {
        List<JobNameRelationEntity> relations = _jobNameRepo.ListAll();

        return relations;
    }
}
