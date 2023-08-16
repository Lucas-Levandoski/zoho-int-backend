


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZohoIntegration.TimeLogs.Enums;
using ZohoIntegration.TimeLogs.Models;
using ZohoIntegration.TimeLogs.Repositories;

namespace ZohoIntegration.TimeLogs.Services;


public class ZohoJobName
{ 
    private readonly ZohoConnection _zohoConnection;
    private readonly JobNameRelation _jobNameRepo;

    public ZohoJobName(
        ZohoConnection zohoConnection,
        JobNameRelation jobNameRelation) 
    {
        _zohoConnection = zohoConnection;
        _jobNameRepo = jobNameRelation;
    }

    public async Task<JobNamesOutput> ListBRJobNames() 
    {
        var results = (await _zohoConnection.GetAsync<JobNamesView>("timetracker/getjobs?assingedTo=all", TargetZohoAccount.BR))?.response?.result;

        if (results is null)
            throw new SystemException("The connection to get the BR Job Names list failed");

        List<JobNameItem> pairs = new();

        foreach(var result in results) 
            pairs.Add(new JobNameItem(){ jobName = result.jobName, jobId = result.jobId, jobProjectId = result.projectId, jobProjectName = result.projectName});

        return new JobNamesOutput() { jobs = pairs };
    }

    public async Task<JobNamesOutput> ListUKJobNames() 
    {
        var results = (await _zohoConnection.GetAsync<JobNamesView>("timetracker/getjobs?assingedTo=all", TargetZohoAccount.UK))?.response?.result;

        if (results is null)
            throw new SystemException("The connection to get the UK Job Names list failed");

        List<JobNameItem> pairs = new();

        foreach(var result in results) 
            pairs.Add(new JobNameItem(){ jobName = result.jobName, jobId = result.jobId, jobProjectId = result.projectId, jobProjectName = result.projectName});

        return new JobNamesOutput() { jobs = pairs };
    }

    public async Task<int> UpdateUKJobDetails(string jobId)
    {
        var result = (await _zohoConnection.GetAsync<JobDetailsView>($"timetracker/getjobdetails?jobId={jobId}", TargetZohoAccount.UK))?.response?.result?.FirstOrDefault();

        if (result is null)
            throw new SystemException("The connection to get the UK Job Details failed");

        var jobs = _jobNameRepo.ListAllJobsByUKId(jobId);
        var newJobs = new List<JobNameRelationEntity>(jobs);

        foreach(var job in jobs)
        {
            newJobs.Add(new JobNameRelationEntity(){
                BRJobName = job.BRJobName,
                BRJobId = job.BRJobId,
                BRJobProjectName = job.BRJobProjectName,
                BRJobProjectId = job.BRJobProjectId,
                UKJobId = job.UKJobId,
                UKJobName = result.jobName,
                UKJobProjectId = result.projectId,
                UKJobProjectName = result.projectName,
                ETag = job.ETag,
                RowKey = job.RowKey,
                PartitionKey = job.PartitionKey
            });
        }
            _jobNameRepo.UpdateOrCreateList(newJobs);
        

        return jobs.Count;
    }

    public async Task<int> UpdateBRJobDetails(string jobId)
    {
        var result = (await _zohoConnection.GetAsync<JobDetailsView>($"timetracker/getjobdetails?jobId={jobId}", TargetZohoAccount.UK))?.response?.result?.FirstOrDefault();

        if (result is null)
            throw new SystemException("The connection to get the UK Job Details failed");

        var jobs = _jobNameRepo.ListAllJobsByBRId(jobId);
        var newJobs = new List<JobNameRelationEntity>(jobs);

        foreach(var job in jobs)
        {
            newJobs.Add(new JobNameRelationEntity(){
                UKJobName = job.UKJobName,
                UKJobId = job.UKJobId,
                UKJobProjectName = job.UKJobProjectName,
                UKJobProjectId = job.UKJobProjectId,
                BRJobId = job.BRJobId,
                BRJobName = result.jobName,
                BRJobProjectId = result.projectId,
                BRJobProjectName = result.projectName,
                ETag = job.ETag,
                RowKey = job.RowKey,
                PartitionKey = job.PartitionKey
            });
        }

        _jobNameRepo.UpdateOrCreateList(newJobs);
        
        return jobs.Count;
    }
}