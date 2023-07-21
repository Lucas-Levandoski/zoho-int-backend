﻿using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using ZohoIntegration.TimeLogs.Models;

namespace ZohoIntegration.TimeLogs.Models;
public class JobNamesRelationOutput
{
    public List<JobNamesRelationItem> relations { get; set; }

    public static implicit operator JobNamesRelationOutput(List<JobNameRelationEntity> entities) 
    {
        List<JobNamesRelationItem> relations = new();

        foreach(var entity in entities)
        {
            JobNameRelationItem ukJobs = new(){
                jobName = entity.UKJobName,
                jobId = entity.UKJobId,
                jobProjectId = entity.UKJobProjectId,
                jobProjectName = entity.UKJobProjectName,
            };

            JobNameRelationItem brJobs = new(){
                jobName = entity.BRJobName,
                jobId = entity.BRJobId,
                jobProjectId = entity.BRJobProjectId,
                jobProjectName = entity.BRJobProjectName,
            };

            relations.Add(new() {
                ukJob = ukJobs,
                brJob = brJobs,
                rowKey = entity.RowKey
            });
        }

        return new() { relations = relations};
    }
}

public class JobNamesRelationItem
{
    public JobNameRelationItem ukJob { get; set; }
    public JobNameRelationItem brJob { get; set; }
    public string rowKey { get; set; }
}

public class JobNameRelationItem {
    public string jobName { get; set; }
    public string jobId { get; set; }
    public string jobProjectId { get; set; }
    public string jobProjectName { get; set; }
}