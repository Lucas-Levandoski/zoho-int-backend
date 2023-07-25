using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
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
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = false, Type = typeof(string), Description = "ID of the job name relation")]
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
    [OpenApiParameter(
        name: "id", 
        In = ParameterLocation.Path, 
        Required = false, 
        Type = typeof(string), 
        Description = "ID of the job name relation"
    )]
    public IActionResult Delete(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "JobName/{id}")] HttpRequest req, 
        string id,
        ILogger log)
    {
        var (currentBRName, currentUKName) = _jobNameService.DeleteRelation(id);

        return new OkObjectResult($"All fine for the delete process, with BR Name {currentBRName} and UK Name {currentUKName}");
    }

    [FunctionName("JobNamesListBR")]
    public async Task<IActionResult> ListBR(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "JobName/list/br")] HttpRequest req,
        ILogger log)
    {
        var result = await _zohoJobNameService.ListBRJobNames();

        return new OkObjectResult(result);
    }

    [FunctionName("JobNamesListUK")]
    public async Task<IActionResult> ListUK(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "JobName/list/uk")] HttpRequest req,
        ILogger log) 
    {
        var result = await _zohoJobNameService.ListUKJobNames();

        return new OkObjectResult(result);
    }

    [FunctionName("JobNamesListAllRelations")]
    public IActionResult ListAllRelations(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "JobName/relations/list")] HttpRequest req,
        ILogger log) 
    {
        var result = _jobNameService.ListAll();

        return new OkObjectResult(result);
    }

    [FunctionName("JobNameRelationUpdateAllBRJobNames")]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = false, Type = typeof(string), Description = "Job ID")]
    public async Task<IActionResult> UpdateBRJobNames(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "JobName/JobDetails/update/Br/{id}")] HttpRequest req,
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

    [FunctionName("JobNameRelationUpdateAllUKJobNames")]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = false, Type = typeof(string), Description = "Job ID")]
    public async Task<IActionResult> UpdateUKJobNames(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "JobName/JobDetails/update/Uk/{id}")] HttpRequest req,
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
}   
