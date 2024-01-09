using System.Collections.Generic;

namespace ZohoIntegration.TimeLogs.Models;
public class AddOrEditTimeLogView
{
        public AddOrEditTimeLogResponse response;
}

public class AddOrEditTimeLogResponse
{
    public List<AddOrEditTimeLogResult> result { get; set; }
    public string message { get; set; }
    public string uri { get; set; }
    public int status { get; set; }
}

public class AddOrEditTimeLogResult
{
    public string timeLogId { get; set; }
}
