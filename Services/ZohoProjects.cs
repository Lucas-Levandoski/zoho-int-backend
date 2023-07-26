using System;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using System.Text;
using ZohoIntegration.TimeLogs.Models;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using ZohoIntegration.TimeLogs.Repositories;
using Azure;

namespace ZohoIntegration.TimeLogs.Services;
public class ZohoProjects
{
    private readonly ZohoConnection _zohoConnection;
    private readonly JobNameRelation _jobNameRepo;

    public ZohoProjects(ZohoConnection zohoConnection, JobNameRelation jobNameRelation)
    {
        _zohoConnection = zohoConnection;
        _jobNameRepo = jobNameRelation;
    }

    public async Task createRelationsByProjectSyncFlag()
    {
        var searchParams = new
        {
            searchField = "SyncToUK",
            searchOperator = "True"
        };

        var formContent = new MultipartFormDataContent();
        formContent.Add(new StringContent(JsonConvert.SerializeObject(searchParams)), "searchParams");

        var result = (await _zohoConnection.PostAsync<ProjectsFormDataView>("forms/P_TimesheetJobsList/getRecords", formContent, Enums.TargetZohoAccount.BR))?.response?.result;

        if(result == null)
            throw new DataException($"After filterying by {searchParams.searchField}, no project was found");

        List<string> projectIds = new();

        foreach(var projectDict in result)
        {
            projectIds.AddRange(projectDict.Keys);
        }

        List<JobNamesResult> jobs = new();

        await Parallel.ForEachAsync(projectIds, body: async (projectId, ct) => {
            var resultJobs = (await _zohoConnection.GetAsync<JobNamesView>($"timetracker/getjobs?assingedTo=all&projectId={projectId}", target: Enums.TargetZohoAccount.BR))?.response?.result;

            if(resultJobs == null)
                return;

            jobs.AddRange(resultJobs);
        });

        var ukJobs = (await _zohoConnection.GetAsync<JobNamesView>($"timetracker/getjobs?assingedTo=all", target: Enums.TargetZohoAccount.UK))?.response?.result;

        if(ukJobs == null)
            throw new DataException("Not able to find any job from UK Zoho");

        var matchingJobsPreview = ukJobs.Join(
            jobs, 
            ukJob => ukJob.jobName, 
            brJob => brJob.jobName, 
            (ukJob, brJob) =>  new JobNameRelationEntity() { 
                BRJobName = brJob.jobName,
                BRJobId = brJob.jobId,
                BRJobProjectName = brJob.projectName,
                BRJobProjectId = brJob.projectId,
                UKJobId = ukJob.jobId ,
                UKJobName = ukJob.jobName,
                UKJobProjectName = ukJob.projectName,
                UKJobProjectId = ukJob.projectId
            })
        .ToList();

        List<JobNameRelationEntity> matchingJobs = new();

        await Parallel.ForEachAsync(matchingJobsPreview, body: async (job, ct) => {
            var (_, relation) = _jobNameRepo.CheckExistingRelationByBRId(job.BRJobId);

            if( relation != null )
            {
                job.ETag = relation.ETag;
                job.RowKey = relation.RowKey;
                job.PartitionKey = relation.PartitionKey;
            }

            matchingJobs.Add(job);
        });

        _jobNameRepo.UpdateOrCreateList(matchingJobs);

        return; 
    }
}
