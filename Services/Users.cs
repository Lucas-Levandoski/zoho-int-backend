using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZohoIntegration.TimeLogs.Repositories;

namespace ZohoIntegration.TimeLogs.Services;
public class Users
{
    private readonly UsersRepo _usersRepo;

    public Users(UsersRepo usersRepo)
    {
        _usersRepo = usersRepo;
    }

    public UsersOutput ListAll()
    {
        var allUsers = _usersRepo.ListAll();

        return new UsersOutput() {
            users = allUsers.Select(user => user.UserMail).ToList(),
            dateTime = DateTime.Today
        };
    }

    public async Task AddList(List<string> uesrs)
    {
        await _usersRepo.SaveListAsync(uesrs);
    }
}
