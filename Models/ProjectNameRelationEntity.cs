using System;
using Azure;
using Azure.Data.Tables;

namespace ZohoIntegration.TimeLogs.Models;
public class ProjectNameRelationEntity : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public string BRProjectName { get; set; }
    public string BRProjectID { get; set; }
    public string UKProjectName { get; set; }
    public string UKProjectID { get; set; }
    public ETag ETag { get; set; }
}
