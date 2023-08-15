using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ZohoIntegration.TimeLogs.Enums;
using ZohoIntegration.TimeLogs.Models;
using ZohoIntegration.TimeLogs.Repositories;

namespace ZohoIntegration.TimeLogs.Services
{
    public class ZohoTimeLogs
    {
        private readonly TimeLogRelation _timeLogRepo;  
        private readonly JobNameRelation _jobNameRepo;  
        private readonly ZohoConnection _zohoConnection;
        private readonly string brAutoText = "From Brazilian Zoho: ";
        private readonly string dateFormat = "dd-MMM-yyyy";

        public ZohoTimeLogs(
            TimeLogRelation timeLogRelation, 
            ZohoConnection zohoConnection,
            JobNameRelation jobNameRelation) 
        {
            _timeLogRepo = timeLogRelation;
            _zohoConnection = zohoConnection;
            _jobNameRepo = jobNameRelation;
        }

        public async Task<TimeLogDetailsView> GetBRTimelogByIDAsync(string id)
        {
            var result = await _zohoConnection.GetAsync<TimeLogDetailsView>($"timetracker/gettimelogdetails?timelogId={id}", target: TargetZohoAccount.BR);

            return result;
        }

        public async Task CreateOnUK(string sourceTimelogId)
        {
            var sourceLog = (await GetBRTimelogByIDAsync(sourceTimelogId)).response.result.FirstOrDefault();

            if (sourceLog == null)
                throw new DataException($"Didn't find the TimeLog Details for the given id {sourceTimelogId}");

            // Check if inputType is 2, which is supposed to be automated type (need to check)
            if (sourceLog.tt_inputType == 2)
                return;

            var targetJob = _jobNameRepo.getByBRId(sourceLog.jobId);

            if(targetJob == null)
                return;

            Dictionary<string, string> queryParams = new (){
                {"user", sourceLog.employeeMailId},
                {"jobId", targetJob.UKJobId},
                {"workDate", sourceLog.workDate},
                {"dateFormat", dateFormat},
                {"hours", sourceLog.hours},
                {"billingStatus", sourceLog.billingStatus},
                {"workItem", $"{brAutoText}{sourceLog.taskName}"},
                {"description", sourceLog.description}
            };

            var postResult = await _zohoConnection.PostAsync<AddOrEditTimeLogView>("timetracker/addtimelog", new FormUrlEncodedContent(queryParams), TargetZohoAccount.UK);
            string newLogId = postResult.response.result?[0].timeLogId ?? throw new DataException("The request failed while trying to create timelog on UK Zoho account");

            _timeLogRepo.SaveRelation(sourceTimelogId, newLogId);
        }


        public async Task UpdateOnUK(string sourceTimelogId)
        {
            var sourceLog = (await GetBRTimelogByIDAsync(sourceTimelogId)).response.result.FirstOrDefault();

            if (sourceLog == null)
                throw new DataException($"Didn't find the TimeLog Details for the given id {sourceTimelogId}");

            var UKTimeLogID = _timeLogRepo.GetByBRTimelogID(sourceTimelogId)?.UKTimeLogID;

            if (UKTimeLogID == null)
                throw new InvalidOperationException($"there is no relation for the BR Timelog Id {sourceTimelogId}");

            
            var targetJob = _jobNameRepo.getByBRId(sourceLog.jobId);

            if(targetJob == null)
                throw new DataException($"Didn't find the Job ID related to the given Job ID {sourceLog.jobId}");

            Dictionary<string, string> queryParams = new (){
                {"timeLogId", UKTimeLogID},
                {"user", sourceLog.employeeMailId},
                {"jobId", targetJob.UKJobId},
                {"workDate", sourceLog.workDate},
                {"dateFormat", dateFormat},
                {"hours", sourceLog.hours},
                {"billingStatus", sourceLog.billingStatus},
                {"workItem", $"{brAutoText}{sourceLog.taskName}"},
                {"description", sourceLog.description}
            };

            var postResult = await _zohoConnection.PostAsync<AddOrEditTimeLogView>("timetracker/edittimelog", new FormUrlEncodedContent(queryParams), TargetZohoAccount.UK);

            if(postResult.response.result is null || postResult.response.result.FirstOrDefault()?.timeLogId != UKTimeLogID )
                throw new DataException($"The request failed while trying to create timelog on UK Zoho account with \n BR Log ID: {sourceTimelogId}\n UK Log ID: {UKTimeLogID}");
        }

        public async Task<string> DeleteOnUKById(string id)
        {
            var targetTimeLogId = _timeLogRepo.GetByBRTimelogID(id).UKTimeLogID;

            await _zohoConnection.PostAsync($"timetracker/deletetimelog?timeLogId={targetTimeLogId}", target: TargetZohoAccount.UK);

            _timeLogRepo.DeleteRelation(id, targetTimeLogId);

            return targetTimeLogId;
        }
    }
}
