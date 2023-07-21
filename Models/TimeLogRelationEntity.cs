using System;
using Azure;
using Azure.Data.Tables;

namespace ZohoIntegration.TimeLogs.Models;
public class TimeLogRelationEntity : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public string BRTimeLogID { get; set; }
    public string UKTimeLogID { get; set; }
    public ETag ETag { get; set; }
}
