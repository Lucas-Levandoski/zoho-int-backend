using System;
using System.Collections.Generic;

namespace ZohoIntegration.TimeLogs.Models;
public class ProjectNamesOutput
{
  public List<ProjectNamePair> projects { get; set; }
}

public class ProjectNamePair
{
  public string projectName { get; set; }
  public string projectId { get; set; }
}
