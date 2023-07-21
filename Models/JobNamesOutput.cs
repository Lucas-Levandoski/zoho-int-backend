using System;
using System.Collections.Generic;

namespace ZohoIntegration.TimeLogs.Models;

public class JobNamesOutput
{
    public List<JobNameItem> jobs { get; set; }
}

public class JobNameItem
{
    public string jobName { get; set; }
    public string jobId { get; set; }
    public string jobProjectName { get; set; }
    public string jobProjectId { get; set; }
}
