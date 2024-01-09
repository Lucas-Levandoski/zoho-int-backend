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

    public UserRelationOutput ListAll() {
        List<UserRelationEntity> relations = _usersRepo.ListAll();

        return relations;
    }
}
