
using System.Collections.Generic;
using ZohoIntegration.TimeLogs.Models;

namespace ZohoIntegration.TimeLogs.Models;
public class UserRelationOutput
{
    public List<UserRelationItem> relations { get; set; }

    public static implicit operator UserRelationOutput(List<UserRelationEntity> entities) 
    {
        List<UserRelationItem> _relations = new();

        foreach(var entity in entities)
        {
            _relations.Add(new() {
                brUserEmail = entity.BRUserMail,
                ukUserEmail = entity.UKUserMail,
                id = entity.RowKey
            });
        }

        return new() { relations = _relations};
    }
}

public class UserRelationItem
{
    public string id { get; set; }
    public string brUserEmail { get; set; }
    public string ukUserEmail { get; set; }
}
