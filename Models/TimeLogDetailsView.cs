
using System.Collections.Generic;

namespace ZohoIntegration.TimeLogs.Models;

public class TimeLogDetailsView
{
    public TimeLogDetailsResponse response;
}

public class TimeLogDetailsResponse
{
    public List<TimeLogDetailsResult> result { get; set; }
    public string message { get; set; }
    public string uri { get; set; }
    public int status { get; set; }
}

public class TimeLogDetailsResult
{
    public string erecno { get; set; }
    public bool timerLog { get; set; }
    public string employeeMailId { get; set; }
    public bool isTimeLogPuhsedToZF { get; set; }
    public string jobColor { get; set; }
    public string description { get; set; }
    public int accuracy { get; set; }
    public string employeeFirstName { get; set; }
    public bool isDeletedAllowed { get; set; }
    public string type { get; set; }
    public string mod_ip { get; set; }
    public string workDate { get; set; }
    public string billedStatus { get; set; }
    public bool jobIsActive { get; set; }
    public string jobName { get; set; }
    public string approvalStatus { get; set; }
    public string hours { get; set; }
    public string db_workDate { get; set; }
    public bool jobIsCompleted { get; set; }
    public int hoursInMins { get; set; }
    public bool isEditAllowed { get; set; }
    public string billingStatus { get; set; }
    public string jobId { get; set; }
    public string add_ip { get; set; }
    public bool isTimelogPushedToQBO { get; set; }
    public int totaltime { get; set; }
    public string employeeLastName { get; set; }
    public string geoLocation { get; set; }
    public string timelogId { get; set; }
    public string taskName { get; set; }
    public int tt_inputType { get; set; }
    public string projectName { get; set; }
    public string projectId { get; set; }
    public bool isPushAllowToZF { get; set; }
}
