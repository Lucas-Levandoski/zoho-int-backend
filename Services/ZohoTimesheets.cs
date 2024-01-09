using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Newtonsoft.Json.Linq;
using ZohoIntegration.TimeLogs.Models;
using ZohoIntegration.TimeLogs.Repositories;

namespace ZohoIntegration.TimeLogs.Services;
public class ZohoTimesheets
{

    private readonly ZohoConnection _zohoConnection;
    private readonly JobNameRelationRepo _jobNameRepo;
    private readonly UsersRepo _usersRepo;
    private readonly string dateFormat = "dd-MMM-yyyy";
    private readonly DateTime today = DateTime.Today;
    private readonly CultureInfo culture;

    public ZohoTimesheets(ZohoConnection zohoConnection, JobNameRelationRepo jobNameRelation, UsersRepo usersRepo)
    {
        _zohoConnection = zohoConnection;
        _jobNameRepo = jobNameRelation;
        _usersRepo = usersRepo;
        culture = CultureInfo.GetCultureInfo("en-US");
    }

    public async Task CreateTimesheets()
    {
        await Parallel.ForEachAsync(_usersRepo.ListAll(), body: async (userEntity, ct) => {
            var jobIds = await FindUKJobIdsByUser(userEntity.UserMail);

            await Parallel.ForEachAsync(jobIds, body: async (jobId, ct) => {
                // filters to only check for existing relations
                var (isExists, relation) = _jobNameRepo.CheckExistingRelationByUKId(jobId);

                if(!isExists) return;
                
                var (newest, oldest) = await CheckNotSubmittedUKTimelogCurrentMonth(userEntity.UserMail, jobId);
                
                if(newest == null || oldest == null) return;

                await CreateTimesheet(userEntity.UserMail, jobId, relation!.UKJobName, newest.Value, oldest.Value);
            });
        });

        return;
    }

    public async Task<List<string>> FindUKJobIdsByUser(string userMail)
    {
        var jobs = ( await _zohoConnection.GetAsync<JobNamesView>($"timetracker/getjobs?assingedTo={userMail}", target: Enums.TargetZohoAccount.UK)).response?.result ;

        if (jobs == null)
            return new();

        return jobs.Select(job => job.jobId).ToList();
    }

    public async Task<(DateTime? newest, DateTime? oldest)> CheckNotSubmittedUKTimelogCurrentMonth(string user, string jobId)
    {
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddSeconds(-1);

        Dictionary<string, string> queryParams = new (){
            {"user", user},
            {"approvalStatus", "unapproved"},
            {"jobId", jobId},
            {"fromDate", firstDayOfMonth.ToString(dateFormat, culture)},
            {"toDate", lastDayOfMonth.ToString(dateFormat, culture)},
            {"dateFormat", dateFormat}
        };

        var result = (await _zohoConnection.GetAsync<TimeLogListView>("timetracker/gettimelogs", queryParams: queryParams, target: Enums.TargetZohoAccount.UK)).response?.result;

        if (result != null && result.Count > 0)
        {
            var oldest = DateTime.ParseExact(result.First().workDate, dateFormat, CultureInfo.InvariantCulture);
            var newest = DateTime.ParseExact(result.Last().workDate, dateFormat, CultureInfo.InvariantCulture);
            return (newest, oldest);
        }

        return (null, null);   
    }

    public async Task CreateTimesheet(string user, string jobId, string jobName, DateTime newestLog, DateTime oldestLog)
    {
        string timesheetName = @$"Schedule Auto Generated timesheet for job({jobName}) on {
            (oldestLog == newestLog ? newestLog.ToString(dateFormat) : $"({oldestLog.ToString(dateFormat)} to {newestLog.ToString(dateFormat)})")
        }";

        Dictionary<string, string> queryParams = new (){
            {"user", user},
            {"timesheetName", timesheetName},
            {"jobId", jobId},
            {"fromDate", oldestLog.ToString(dateFormat, culture)},
            {"toDate", newestLog.ToString(dateFormat, culture)},
            {"dateFormat", dateFormat},
            {"sendforApproval", "true"}
        };

        await _zohoConnection.PostAsync("timetracker/createtimesheet", content: new FormUrlEncodedContent(queryParams), target: Enums.TargetZohoAccount.UK);
    }
}
