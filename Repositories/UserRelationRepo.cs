using System;
using System.Collections.Generic;
using System.Linq;
using Azure;
using Azure.Data.Tables;
using ZohoIntegration.TimeLogs.Models;
using ZohoIntegration.TimeLogs.Repositories;

namespace ZohoIntegration.TimeLogs;
public class UserRelationRepo
{
    internal readonly StorageAccountTableConnection _tableConnection;
    internal readonly TableClient _tableClient;
    internal readonly string _partitionKey = "userRelation";

    public UserRelationRepo(StorageAccountTableConnection tableConnection)
    {
        _tableConnection = tableConnection;
        _tableClient = tableConnection.GetTableClient("UserRelation");
    }

    public List<UserRelationEntity> ListAll()
    {
        var result = _tableClient.Query<UserRelationEntity>().ToList();

        return result;
    }

    public UserRelationEntity? GetByMails(string BRUserMail, string UKUserMail)
    {
        var result = _tableClient.Query<UserRelationEntity>(filter: table => table.BRUserMail == BRUserMail && table.UKUserMail == UKUserMail).FirstOrDefault();

        return result;
    }

    public UserRelationEntity SaveRelation(string BRUserMail, string UKUserMail, string? partitionKey = null)
    {
        var entity = new UserRelationEntity() {
            PartitionKey = partitionKey ?? _partitionKey,
            BRUserMail = BRUserMail,
            UKUserMail = UKUserMail,
            RowKey = Guid.NewGuid().ToString(),
            ETag = new ETag()
        };

        _tableClient.AddEntity(entity);

        return entity;
    }



    public bool TrySaveRelation(string BRUserMail, string UKUserMail)
    {
        var entity = GetByMails(BRUserMail, UKUserMail);

        if(entity != null) {
            return false;
        }

        SaveRelation(BRUserMail, UKUserMail);
        return true;
    }
}
