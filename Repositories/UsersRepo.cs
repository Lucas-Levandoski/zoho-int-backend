using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using ZohoIntegration.TimeLogs.Models;

namespace ZohoIntegration.TimeLogs.Repositories;
public class UsersRepo
{
    internal readonly StorageAccountTableConnection _tableConnection;
    internal readonly TableClient tableClient;
    internal readonly string partitionKey = "users";

    public UsersRepo(StorageAccountTableConnection tableConnection)
    {
        _tableConnection = tableConnection;
        tableClient = tableConnection.GetTableClient("UsersList");
    }

    public async Task SaveListAsync(List<string> users) 
    {
        var userEntities = PopulateUserEntities(users);
        
        List<TableTransactionAction> addUsers = new List<TableTransactionAction>();
        addUsers.AddRange(userEntities.Select(user => new TableTransactionAction(TableTransactionActionType.Add, user)));
        var response = await tableClient.SubmitTransactionAsync(addUsers);
    }

    public List<UserEntity> ListAll()
    {
        var result = tableClient.Query<UserEntity>().ToList();

        return result;
    }

    public async Task ReplaceAll(List<string> users) 
    {
        List<TableTransactionAction> addUsers = new List<TableTransactionAction>();
        List<TableTransactionAction> removeUsers = new List<TableTransactionAction>();

        var toDelete = ListAll();
        var toAdd = PopulateUserEntities(users);

        removeUsers.AddRange(toDelete.Select(user => new TableTransactionAction(TableTransactionActionType.Delete, user)));
        var deleteResult = await tableClient.SubmitTransactionAsync(removeUsers);

        addUsers.AddRange(toAdd.Select(user => new TableTransactionAction(TableTransactionActionType.Delete, user)));
        var addResult = await tableClient.SubmitTransactionAsync(addUsers);
    }

    private List<UserEntity> PopulateUserEntities(List<string> users) 
    {
        return users.Select(user => {
            return new UserEntity
                {
                    PartitionKey = partitionKey,
                    RowKey = Guid.NewGuid().ToString(),
                    ETag = new ETag(),
                    UserMail = user,
                };
        }).ToList();
    }

}
