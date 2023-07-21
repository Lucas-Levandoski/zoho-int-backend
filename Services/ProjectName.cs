using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZohoIntegration.TimeLogs.Models;
using ZohoIntegration.TimeLogs.Repositories;
using ZohoIntegration.TimeLogs.Services;

namespace ZohoIntegration.TimeLogs.Services;
public class ProjectName
{
  private readonly ProjectNameRelation _projectNameRepo;
  private readonly ZohoConnection _zohoConnection;

  public ProjectName(ProjectNameRelation projectNameRelation, ZohoConnection zohoConnection) 
  {
    _projectNameRepo = projectNameRelation;
    _zohoConnection = zohoConnection;
  }

  public void CreateRelation(string brName, string brId, string ukName, string ukId) 
  {
    var (isExists, _) = _projectNameRepo.CheckExistingRelation(brName, ukName);

    if(isExists) 
    {
      throw new InvalidOperationException("This relation already exists, please define new names or update this relation");
    }

    _projectNameRepo.SaveRelation(brName, brId, ukName, ukId); 
  }

  public (string oldBRName, string oldUKName) UpdateRelation(string brName, string brId, string ukName, string ukId, string rowKey) 
  {
    var (isExists, oldBRName, oldUKName, partitionKey) = _projectNameRepo.CheckExistingRelation(rowKey);

    if(!isExists) 
    {
      throw new InvalidOperationException($"Relation ID {rowKey} not found");
    }

    WriteLine("Changing names from BRNames: {0} -> {1} and UKNames: {2} -> {3}", oldBRName, brName, oldUKName, ukName);

    _projectNameRepo.DeleteRelation(partitionKey, rowKey);

    _projectNameRepo.SaveRelation(brName, brId, ukName, ukId, rowKey);

    return (oldBRName, oldUKName);
  }

}
