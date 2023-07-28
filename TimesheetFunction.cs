using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using ZohoIntegration.TimeLogs.Services;

namespace ZohoIntegration.TimeLogs;
public class TimesheetFunction
{

    private readonly ZohoTimesheets _timesheetService;

    public TimesheetFunction(ZohoTimesheets zohoTimesheets)
    {
        _timesheetService = zohoTimesheets;
    }

    [FunctionName("TimesheetDailyCreate")]
    public async Task<IActionResult> DailyCreate(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "CreateUKTimesheets")] HttpRequest req,
        ILogger log)
    {
        await _timesheetService.CreateTimesheets();

        return new OkObjectResult("work in progress");
    }
}
