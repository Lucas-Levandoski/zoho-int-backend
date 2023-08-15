using System;
using System.Collections.Generic;
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
public class UsersFunction
{
    
    private readonly Users _usersService;

    public UsersFunction(Users readTimeService)
    {
        _usersService = readTimeService;
    }

    [FunctionName("UsersListAll")]
    public IActionResult ListAll(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Users")] HttpRequest req,
        ILogger log)
    {
        var result = _usersService.ListAll();

        return new OkObjectResult(result);
    }

    [FunctionName("UsersAddList")]
    public async Task<IActionResult> AddList(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Users")] HttpRequest req,
        ILogger log)
    {
        
        UsersInput body;
        using (var streamReader = new StreamReader(req.Body))
        {
            string requestBody = await streamReader.ReadToEndAsync();
            body = JsonConvert.DeserializeObject<UsersInput>(requestBody) ?? throw new ArgumentException("Missing request body property");       
        } 

        await _usersService.AddList(body.users);

        return new OkObjectResult("all added");
    } 
}
