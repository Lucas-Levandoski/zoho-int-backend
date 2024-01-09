using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using ZohoIntegration.TimeLogs.Services;

namespace ZohoIntegration.TimeLogs;
public class UserRelationFunction
{
    private readonly UserRelation _userRelationService;

    public UserRelationFunction(UserRelation readTimeService)
    {
        _userRelationService = readTimeService;
    }

    [FunctionName("UserRelationAdd")]
    public async Task<IActionResult> AddRelation(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "UserEmailRelation")] HttpRequest req,
        ILogger log)
    {
        return new OkObjectResult("all fine");
    }

    [FunctionName("UserRelationListAll")]
    public async Task<IActionResult> ListAll(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "UserEmailRelation")] HttpRequest req,
        ILogger log)
    {
        var result = _userRelationService.ListAll();

        return new OkObjectResult(result);
    }
}
