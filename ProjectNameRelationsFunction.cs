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
using ZohoIntegration.TimeLogs.Services;
namespace ZohoIntegration.TimeLogs;
public class ProjectNameRelationsFunction
{
    private readonly ProjectName _projectNameService;
    private readonly ZohoProjectName _zohoProjectNameService;

    public ProjectNameRelationsFunction(ProjectName projectName, ZohoProjectName zohoProjectName)
    {
        _projectNameService = projectName;
        _zohoProjectNameService = zohoProjectName;
    }
    
    public class ProjectNamesBody {
        public string brName { get; set; } = "";
        public string brId { get; set; } = "";
        public string ukName { get; set; } = "";
        public string ukId { get; set; } = "";
    }


    [FunctionName("ProjectNameRelationCreate")]
    public async Task<IActionResult> Create(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "ProjectName")] HttpRequest req,
        ILogger log)
    {
        try {

            ProjectNamesBody? names;
            using (var streamReader = new StreamReader(req.Body))
            {
                string requestBody = await streamReader.ReadToEndAsync() ?? throw new ArgumentException("Missing request body");
                names = JsonConvert.DeserializeObject<ProjectNamesBody>(requestBody);       
            }

            _projectNameService.CreateRelation(
                names?.brName ?? throw new ArgumentException("Missing brName property"), 
                names?.brId ?? throw new ArgumentException("Missing brId property"), 
                names?.ukName ?? throw new ArgumentException("Missing ukName property"), 
                names?.ukId ?? throw new ArgumentException("Missing ukId property")
            );
            return new OkObjectResult($"all fine for the create ({names.brName} - {names.ukName})");
    
        } catch (ArgumentException ex) {
            return new BadRequestObjectResult(ex.Message);
        } catch (Exception ex) {
            return new ObjectResult(ex.Message) { StatusCode = 500 };
        }

    }


    [FunctionName("ProjectNameRelationUpdate")]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = false, Type = typeof(string), Description = "ID of the project name relation")]
    public async Task<IActionResult> Update(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "ProjectName/{id}")] HttpRequest req,
        string id,
        ILogger log)
    {
        try {
            ProjectNamesBody? names;
            using (var streamReader = new StreamReader(req.Body))
            {
                string requestBody = await streamReader.ReadToEndAsync();
                names = JsonConvert.DeserializeObject<ProjectNamesBody>(requestBody);       
            } 

            var updatedResult = _projectNameService.UpdateRelation(
                names?.brName ?? throw new ArgumentException("Missing brName property"), 
                names?.brId ?? throw new ArgumentException("Missing brId property"), 
                names?.ukName ?? throw new ArgumentException("Missing ukName property"), 
                names?.ukId ?? throw new ArgumentException("Missing ukId property"), 
                id ?? throw new ArgumentException("Missing ID Path Parameter property")
            );

            return new OkObjectResult(
                $"all fine for the update on ID {id}, with changes (BRNames: {names.brName} -> {updatedResult.oldBRName} and UKNames: {names.ukName} -> {updatedResult.oldUKName})"
            );
        } catch (ArgumentException ex) {
            return new BadRequestObjectResult(ex.Message);
        } catch (Exception ex) {
            return new ObjectResult(ex.Message) { StatusCode = 500 };
        }
    }

    [FunctionName("ProjectNamesListBR")]
    public async Task<IActionResult> ListBR(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ProjectName/list/br")] HttpRequest req,
        ILogger log)
    {
        var result = await _zohoProjectNameService.ListBRProjectNames();

        return new OkObjectResult(result);
    }

    [FunctionName("ProjectNamesListUK")]
    public async Task<IActionResult> ListUK(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ProjectName/list/uk")] HttpRequest req,
        ILogger log) 
    {
        var result = await _zohoProjectNameService.ListUKProjectNames();

        return new OkObjectResult(result);
    }
}
