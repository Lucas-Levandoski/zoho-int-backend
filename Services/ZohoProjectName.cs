


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZohoIntegration.TimeLogs.Enums;
using ZohoIntegration.TimeLogs.Models;

namespace ZohoIntegration.TimeLogs.Services;


public class ZohoProjectName
{ 
    private readonly ZohoConnection _zohoConnection;

    public ZohoProjectName(
        ZohoConnection zohoConnection) 
    {
        _zohoConnection = zohoConnection;
    }

    public async Task<ProjectNamesOutput> ListBRProjectNames() 
    {
        var results = (await _zohoConnection.GetAsync<ProjectNamesView>("timetracker/getprojects", TargetZohoAccount.BR))?.response?.result;

        if (results is null)
          throw new SystemException("The connection to get the BR Project Names list failed");

        List<ProjectNamePair> pairs = new();

        foreach(var result in results) 
          pairs.Add(new ProjectNamePair(){ projectName = result.projectName, projectId = result.projectId});


        return new ProjectNamesOutput() { projects = pairs };
    }

    public async Task<ProjectNamesOutput> ListUKProjectNames() 
    {
        var results = (await _zohoConnection.GetAsync<ProjectNamesView>("timetracker/getprojects", TargetZohoAccount.UK))?.response?.result;
        if (results is null)
          throw new SystemException("The connection to get the BR Project Names list failed");

        List<ProjectNamePair> pairs = new();

        foreach(var result in results) 
          pairs.Add(new ProjectNamePair(){ projectName = result.projectName, projectId = result.projectId});


        return new ProjectNamesOutput() { projects = pairs };
    }
}