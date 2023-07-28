using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ZohoIntegration.TimeLogs.Services;
using System;
using System.Data;

namespace ZohoIntegration.TimeLogs;

public class TimeLogFunction
{
    private readonly ZohoTimeLogs _timeLogService;

    public TimeLogFunction(ZohoTimeLogs readTimeService)
    {
        _timeLogService = readTimeService;
    }

    /*
        <summary>
            This post method has no req.body, zoho only sends information through the query params.
        </summary>
    */
    [FunctionName("TimeLogCreate")]
    public async Task<IActionResult> Create(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Create/{id}")] HttpRequest req,
        string id,
        ILogger log)
    {
        try {
            await _timeLogService.CreateOnUK(id);
        } 
        catch (DataException ex) {
            return new BadRequestObjectResult($"Request failed with the following message: \n --- {ex.Message}");
        }
        catch (Exception ex) {
            log.LogError("Request failed with the following message: --'{0}'-- and the following stacktrace:\n----\n{1}", ex.Message, ex.StackTrace);
            throw;
        }

        return new OkObjectResult("Time Log successfully created on UK Zoho Account");
    }

    /*
        <summary>
            This post method has no req.body, zoho only sends information through the query params.
        </summary>
    */
    [FunctionName("TimeLogEdit")]
    public async Task<IActionResult> Edit(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Edit/{id}")] HttpRequest req,
        string id,
        ILogger log)
    {
        try {
            await _timeLogService.UpdateOnUK(id);
        } 
        catch (DataException ex) {
            return new BadRequestObjectResult($"Request failed with the following message: \n --- {ex.Message}");
        }
        catch (InvalidOperationException ex) {
            // return a Accepted 202 object result meaning that nothing was actually done but it didn't fail
            return new ObjectResult($"the BR Timelog ID -- {id} -- has no related UK Timelog ID, Exception Message: {ex.Message}") { StatusCode = 202 };
        }
        catch (Exception ex) {  
            log.LogError("Request failed with the following message: --'{0}'-- and the following stacktrace:\n----\n{1}", ex.Message, ex.StackTrace);
            throw;
        }

        return new OkObjectResult("Time Log successfully edited on UK Zoho Account");
    }

    /*
        <summary>
            This Delete method is a post due to how Zoho runs the delete webhook, it only triggers a post or a get.
        </summary>
    */
    [FunctionName("TimeLogDelete")]
    public async Task<IActionResult> Delete(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Delete/{id}")] HttpRequest req,
        string id,
        ILogger log)
    {
        try {
            var ukTimelogId = await _timeLogService.DeleteOnUKById(id);
            return new OkObjectResult($"Timelogs deleted with the following IDs: \nBR - {id}\nUK - {ukTimelogId}");
        } 
        catch (InvalidOperationException ex) {
            // return a Accepted 202 object result meaning that nothing was actually done but it didn't fail
            return new ObjectResult($"the BR Timelog ID -- {id} -- has no related UK Timelog ID, Exception Message: {ex.Message}") { StatusCode = 202 };
        }
        catch (Exception ex) {
            log.LogError("Request failed with the following message: --'{0}'-- and the following stacktrace:\n----\n{1}", ex.Message, ex.StackTrace);
            throw;
        }
    }

    [FunctionName("TimeLogRead")]
    public async Task<IActionResult> Read(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Read/{id}")] HttpRequest req,
        string id,
        ILogger log)
    {
        await _timeLogService.GetBRTimelogByIDAsync(id);

        return new OkObjectResult("time read");
    }
}
