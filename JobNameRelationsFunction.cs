using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ZohoIntegration.TimeLogs.Models;
using ZohoIntegration.TimeLogs.Services;

namespace ZohoIntegration.TimeLogs;
public class JobNameRelationsFunction
{
    private readonly JobName _jobNameService;
    private readonly ZohoJobName _zohoJobNameService;

    public JobNameRelationsFunction(JobName jobName, ZohoJobName zohoJobName)
    {
        _jobNameService = jobName;
        _zohoJobNameService = zohoJobName;
    }
    
    [FunctionName("JobNameRelationCreate")]
    public async Task<IActionResult> Create(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "JobName")] HttpRequest req,
        ILogger log)
    {
        JobNameRelationInput names;
        using (var streamReader = new StreamReader(req.Body))
        {
            string requestBody = await streamReader.ReadToEndAsync();
            names = JsonConvert.DeserializeObject<JobNameRelationInput>(requestBody) ?? throw new ArgumentException("Missing request body property");       
        }

        try {
            _jobNameService.CreateRelation(names);
        } catch(InvalidOperationException ex) {
            return new BadRequestObjectResult(ex.Message);
        }

        return new OkObjectResult($"All fine for the create ({names.brJobName} - {names.ukJobName})");
    }


    [FunctionName("JobNameRelationUpdate")]
    public async Task<IActionResult> Update(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "JobName/{id}")] HttpRequest req,
        string id,
        ILogger log)
    {
        JobNameRelationInput names;
        using (var streamReader = new StreamReader(req.Body))
        {
            string requestBody = await streamReader.ReadToEndAsync();
            names = JsonConvert.DeserializeObject<JobNameRelationInput>(requestBody) ?? throw new ArgumentException("Missing request body property");       
        } 

        try {
            var (oldBRName, oldUKName) = _jobNameService.UpdateRelation(names, id);

            return new OkObjectResult(
                $"All fine for the update with changes (BRNames: {names.brJobName} -> {oldBRName} and UKNames: {names.ukJobName} -> {oldUKName})"
            );
        } catch(InvalidOperationException ex) {
            return new BadRequestObjectResult(ex.Message);
        }
    }


    [FunctionName("JobNameRelationDelete")]
    public IActionResult Delete(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "JobName/{id}")] HttpRequest req, 
        string id,
        ILogger log)
    {
        var (currentBRName, currentUKName) = _jobNameService.DeleteRelation(id);

        return new OkObjectResult($"All fine for the delete process, with BR Name {currentBRName} and UK Name {currentUKName}");
    }

    [FunctionName("JobNamesListAllRelations")]
    public IActionResult ListAllRelations(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "JobName/relations/list")] HttpRequest req,
        ILogger log) 
    {
        var result = _jobNameService.ListAll();

        return new OkObjectResult(result);
    }
}   
