using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ZohoIntegration.TimeLogs.Models;
public class ProjectsFormDataView
{
    public ProjectsFormDataResponse response;
}

public class ProjectsFormDataResponse
{
    /*
        this result can`t be specified by type directly because the response property name is the ID, so it it is dynamic
        e.g.
        [{
            "123123": {
                "Project_Name": "Name 1"
            }
        }]

        where 123123 is the project ID
    */
    
    public List<Dictionary<string, List<ProjectsFormDataResult>>> result { get; set; }
    public string message { get; set; }
    public string uri { get; set; }
    public int status { get; set; }
}

public class ProjectsFormDataResultItem
{
    public ProjectsFormDataResult Data { get; set; }
}

public class ProjectsFormDataResult
{
    [JsonProperty(PropertyName = "CreatedTime")]
    public string createdTime { get; set; }	


    [JsonProperty(PropertyName = "ProjectHead.ID")]
    public string projectHeadId { get; set; }


    [JsonProperty(PropertyName = "AddedTime")]
    public string addedTime { get; set; }


    [JsonProperty(PropertyName = "SyncToUK")]
    public string syncToUK { get; set; }

    [JsonProperty(PropertyName = "Project_Name")]
    public string projectName { get; set; }

    
    [JsonProperty(PropertyName = "ProjectHead.details")]
    public List<JObject> projectHeadDetails { get; set; }


    [JsonProperty(PropertyName = "ModifiedBy")]
    public string modifiedBy { get; set; }


    [JsonProperty(PropertyName = "ApprovalStatus")]
    public string approvalStatus { get; set; }


    [JsonProperty(PropertyName = "ModifiedTime")]
    public string ModifiedTime { get; set; }


    [JsonProperty(PropertyName = "ProjectOwner")]
    public string projectOwner { get; set; }


    [JsonProperty(PropertyName = "Zoho_ID")]
    public string projectId { get; set; }


    [JsonProperty(PropertyName = "ClientId")]
    public string clientId { get; set; }


    [JsonProperty(PropertyName = "ProjectCost.displayValue")]
    public string projectCostValue { get; set; }


    [JsonProperty(PropertyName = "ProjectOwner.ID")]
    public string projectOwnerId { get; set; }


    
    [JsonProperty(PropertyName = "ProjectDescription")]
    public string projectDescription { get; set; }


    
    [JsonProperty(PropertyName = "AddedBy")]
    public string addedBy { get; set; }


    
    [JsonProperty(PropertyName = "Status")]
    public string status { get; set; }


    
    [JsonProperty(PropertyName = "ProjectHead")]
    public string projectHead { get; set; }

    
    [JsonProperty(PropertyName = "Status.displayValue")]
    public string statusValue { get; set; }

    
    [JsonProperty(PropertyName = "ProjectManager")]
    public string projectManager { get; set; }


    // no idea why this is a string but that is how zoho delivers
    [JsonProperty(PropertyName = "ProjectUsers")]
    public string projectUsers { get; set; }


    
    [JsonProperty(PropertyName = "ModifiedBy.ID")]
    public string modifiedById { get; set; }


    
    [JsonProperty(PropertyName = "ClientId.ID")]
    public string clientId_Id { get; set; }


    
    [JsonProperty(PropertyName = "AddedBy.ID")]
    public string addedById { get; set; }


    
    [JsonProperty(PropertyName = "ProjectCost")]
    public string projectCost { get; set; }


    
    [JsonProperty(PropertyName = "componentValueDisp")]
    public string componentValueDisplay { get; set; }
}