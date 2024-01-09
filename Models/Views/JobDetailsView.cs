using System;
using System.Collections.Generic;

namespace ZohoIntegration.TimeLogs.Models;
public class JobDetailsView
{
    public JobDetailsResponse? response;
}

public class JobDetailsResponse
{
    public List<JobDetailsResult>? result { get; set; }
    public string message { get; set; }
    public string uri { get; set; }
    public int status { get; set; }
}

public class JobDetailsResult
{
    public string jobName { get; set; }
    public string owner { get; set; }
    public string jobStatus { get; set; }
    public string hours { get; set; }
    public string assignedBy { get; set; }
    public List<object> assignees { get; set; }
    public bool isDeleteAllowed { get; set; }
    public string projectHead { get; set; }
    public bool isActive { get; set; }
    public string fromDate { get; set; }
    public string jobId { get; set; }
    public int jobcolor { get; set; }
    public int ratePerHour { get; set; }
    public string totalhours { get; set; }
    public string projectName { get; set; }
    public string projectId { get; set; }
}