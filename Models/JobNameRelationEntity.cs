using System;
using Azure;
using Azure.Data.Tables;

namespace ZohoIntegration.TimeLogs.Models;
public class JobNameRelationEntity : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public string BRJobName { get; set; }
    public string BRJobId { get; set; }
    public string BRJobProjectName { get; set; }
    public string BRJobProjectId { get; set; }
    public string UKJobName { get; set; }
    public string UKJobId { get; set; }
    public string UKJobProjectName { get; set; }
    public string UKJobProjectId { get; set; }
    public ETag ETag { get; set; }
}
