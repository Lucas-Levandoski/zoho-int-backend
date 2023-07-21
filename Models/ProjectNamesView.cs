using System;
using System.Collections.Generic;

namespace ZohoIntegration.TimeLogs.Models;
public class ProjectNamesView
{
    public ProjectNamesResponse? response;
}

public class ProjectNamesResponse
{
    public List<ProjectNamesResult>? result { get; set; }
    public string message { get; set; }
    public string uri { get; set; }
    public int status { get; set; }
}

public class ProjectNamesResult 
{
    public string projectStatus { get; set; }
    public string ownerName { get; set; }
    public bool isDeleteAllowed { get; set; }
    public string projectName { get; set; }
    public object projectHead { get; set; }
    public string ownerId { get; set; }
    public string projectId { get; set; }
}