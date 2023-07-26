using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using ZohoIntegration.TimeLogs.Services;

namespace ZohoIntegration.TimeLogs;
public class JobNameZohoMappingFunction
{

    private readonly ZohoJobName _zohoJobNameService;
    private readonly ZohoProjects _zohoProjectsService;

    public JobNameZohoMappingFunction(ZohoJobName zohoJobNameService, ZohoProjects zohoProjectsService) 
    {
        _zohoJobNameService = zohoJobNameService;
        _zohoProjectsService = zohoProjectsService;
    }


    [FunctionName("JobNamesZohoMappingListBR")]
    public async Task<IActionResult> ListBR(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "JobName-zoho/list/br")] HttpRequest req,
        ILogger log)
    {
        var result = await _zohoJobNameService.ListBRJobNames();

        return new OkObjectResult(result);
    }


    [FunctionName("JobNamesZohoMappingListUK")]
    public async Task<IActionResult> ListUK(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "JobName-zoho/list/uk")] HttpRequest req,
        ILogger log) 
    {
        var result = await _zohoJobNameService.ListUKJobNames();

        return new OkObjectResult(result);
    }


    [FunctionName("JobNamesZohoMappingUpdateAllBRJobNames")]
    public async Task<IActionResult> UpdateBRJobNames(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "JobName-zoho/JobDetails/update/Br/{id}")] HttpRequest req,
        string id,
        ILogger log)
    {
        try {
            int count = await _zohoJobNameService.UpdateBRJobDetails(id);

            return new OkObjectResult(
                $"Updated {count} BR job relations"
            );
        } catch(InvalidOperationException ex) {
            return new BadRequestObjectResult(ex.Message);
        }
    }


    [FunctionName("JobNamesZohoMappingUpdateAllUKJobNames")]
    public async Task<IActionResult> UpdateUKJobNames(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "JobName-zoho/JobDetails/update/Uk/{id}")] HttpRequest req,
        string id,
        ILogger log)
    {
        try {
            int count = await _zohoJobNameService.UpdateUKJobDetails(id);

            return new OkObjectResult(
                $"Updated {count} UK job relations"
            );
        } catch(InvalidOperationException ex) {
            return new BadRequestObjectResult(ex.Message);
        }
    }


    [FunctionName("JobNamesZohoMappingPopulateMatchingJobs")]
    public async Task<IActionResult> PopulateMatchingJobs(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "JobName-zoho/populate-matching-jobs")] HttpRequest req,
        ILogger log)
    {
        try {
            await _zohoProjectsService.createRelationsByProjectSyncFlag();

            return new OkObjectResult(
                $"Updated UK job relations"
            );
        } catch(InvalidOperationException ex) {
            return new BadRequestObjectResult(ex.Message);
        }
    }

}
