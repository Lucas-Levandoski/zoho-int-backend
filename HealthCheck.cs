using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ZohoIntegration.TimeLogs.Repositories;
using System.Web.Http;

namespace ZohoIntegration.TimeLogs;

public class HealthCheck
{
    private readonly StorageAccountTableConnection _saConnection;
    public HealthCheck(StorageAccountTableConnection SAConnection)
    {
        _saConnection = SAConnection;
    }

    [FunctionName("CustomHealthCheck")]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        try {
            _saConnection.CheckConnection();
        } catch (Exception e) {
            WriteLine(e);
            return new BadRequestErrorMessageResult("Storage Account connection is not working 😣");
        }

        return new OkObjectResult("all fine!! 😊");
    }
}
