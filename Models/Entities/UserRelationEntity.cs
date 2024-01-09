using System;
using Azure;
using Azure.Data.Tables;

namespace ZohoIntegration.TimeLogs.Models;
public class UserRelationEntity : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public string BRUserMail { get; set; }
    public string UKUserMail { get; set; }
    public ETag ETag { get; set; }
}
