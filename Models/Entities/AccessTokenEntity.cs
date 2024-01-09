using System;
using Azure;
using Azure.Data.Tables;

namespace ZohoIntegration.TimeLogs.Models;
public class AccessTokenEntity : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public string CurrentAccessToken { get; set; }
    public ETag ETag { get; set; }
}
