using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sqlservermanagementobjects.Common
{
    public static class JobStepExtensionMethods
    {
        public static JobStep GetLastFailedStep(this Job failedJob)
        {
            if (failedJob == null)
                throw new ArgumentNullException("failedJob cannot be null");

            if (failedJob.JobSteps == null || failedJob.JobSteps.Count == 0)
                throw new ArgumentNullException("failedJob steps cannot be null or 0");


            for (int i = (failedJob.JobSteps.Count-1); i >= 0; i--)
            {
                if (failedJob.JobSteps[i].LastRunOutcome == CompletionResult.Failed & failedJob.JobSteps[i].LastRunDate > DateTime.MinValue)
                    return failedJob.JobSteps[i];
            }
            
            return null;
        }

        public static string GetLastFailedMessageFromJob(this Job failedJob)
        {
            if (failedJob == null)
                throw new ArgumentNullException("failedJob cannot be null");

            JobStep failedStep = failedJob.GetLastFailedStep();

            if (failedStep == null)
                throw new ArgumentNullException("No failed step found for job " + failedJob.Name);
            
            JobHistoryFilter filter = new JobHistoryFilter()
            {
                JobID = failedJob.JobID,
                OldestFirst = false,
                OutcomeTypes = CompletionResult.Failed

            };

            var jobHistory = failedJob.EnumHistory(filter).Select("StepName='" + failedStep.Name + "'", "");

            if (jobHistory != null && jobHistory.Length > 0)
                return jobHistory[0]["Message"].ToString();

            return string.Empty;
        }
    }
}
