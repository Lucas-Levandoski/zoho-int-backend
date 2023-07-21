using System;
using Azure.Data.Tables;

namespace ZohoIntegration.TimeLogs.Repositories;
public class StorageAccountTableConnection
{
    public TableServiceClient tableServiceClient;

    public StorageAccountTableConnection() {
        string accountKey = Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_KEY")  ?? throw new SystemException("Missing STORAGE_ACCOUNT_KEY env var");
        string accountName = Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_NAME") ?? throw new SystemException("Missing STORAGE_ACCOUNT_NAME env var");
        string uri = Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_URI") ?? throw new SystemException("Missing STORAGE_ACCOUNT_URI env var");

        tableServiceClient =
            new TableServiceClient(
                new Uri(uri),
                new TableSharedKeyCredential(accountName, accountKey)
            );
    }

    public TableClient GetTableClient(string tableName) {
        tableServiceClient.CreateTableIfNotExists(tableName);
        return tableServiceClient.GetTableClient(tableName);
    }

}
