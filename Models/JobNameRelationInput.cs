using System;

namespace ZohoIntegration.TimeLogs.Models;
public class JobNameRelationInput
{
    public string brJobName { get; set; }
    public string brJobId { get; set; }
    public string brJobProjectId { get; set; }
    public string brJobProjectName { get; set; }
    public string ukJobName { get; set; }
    public string ukJobId { get; set; }
    public string ukJobProjectId { get; set; }
    public string ukJobProjectName { get; set; }
}
