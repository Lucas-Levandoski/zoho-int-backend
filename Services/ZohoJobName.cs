


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZohoIntegration.TimeLogs.Enums;
using ZohoIntegration.TimeLogs.Models;

namespace ZohoIntegration.TimeLogs.Services;


public class ZohoJobName
{ 
    private readonly ZohoConnection _zohoConnection;

    public ZohoJobName(
        ZohoConnection zohoConnection) 
    {
        _zohoConnection = zohoConnection;
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
            throw new SystemException("The connection to get the BR Job Names list failed");

        List<JobNameItem> pairs = new();

        foreach(var result in results) 
            pairs.Add(new JobNameItem(){ jobName = result.jobName, jobId = result.jobId, jobProjectId = result.projectId, jobProjectName = result.projectName});

        return new JobNamesOutput() { jobs = pairs };
    }
}