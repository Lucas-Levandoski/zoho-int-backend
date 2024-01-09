using System;
using System.Collections.Generic;
using ZohoIntegration.TimeLogs.Models;

namespace ZohoIntegration.TimeLogs;
public class UserRelation
{

    private readonly UserRelationRepo _usersRepo;

    public UserRelation(UserRelationRepo usersRepo)
    {
        _usersRepo = usersRepo;
    }

    public List<UserRelationEntity> ListAll() {
        return _usersRepo.ListAll();
    }
}
