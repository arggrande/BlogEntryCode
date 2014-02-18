using System;
using Microsoft.SqlServer.Management.Smo;
using System.Threading;
using Microsoft.SqlServer.Management.Smo.Agent;
using sqlservermanagementobjects.Common;
using System.Data;
using System.Threading.Tasks;
namespace sqlservermanagementobjects
{
    class Program
    {
        const string _server = "(local)";       // this will only work if you are running a default local sql instance, otherwise substitute your given server here.
        const string _job = "MyAwesomeJob";
        const int _interval = 1000;
        static void Main(string[] args)
        {
            
            try
            {
                WriteInfo("Connecting to " + _server + " instance of SQL Server 2008...");

                Server server = new Server(_server);

                if (server.ConnectionContext.IsOpen)
                {
                    WriteError("Oh no, something weird happened! The Server object shouldnt be opened at this point!");
                    return;
                }
                WriteInfo("Server object is active (but not connected)!");

                WriteInfo("Retrieving '" + _job + "' SQL Job on the local SQL Agent.");

                var job = server.JobServer.Jobs[_job];

                WriteInfo("Job object is active.");

                var lastRunDate = job.LastRunDate;

                job.Start();
                job.Refresh();


                // The job.LastRunDate will only change once the job finishes executing, providing a way of monitoring the job
                while (job.LastRunDate == lastRunDate)
                {
                    Thread.Sleep(_interval);
                    job.Refresh();
                }

                if (job.LastRunOutcome == CompletionResult.Succeeded)
                    WriteInfo("Hooray! Job Successful");
                else
                {
                    WriteError("Oh Noes! Job failed!");
                    WriteError("Job failed with message: " + job.GetLastFailedMessageFromJob());
                }
            }
            catch(Exception ex)
            {
                WriteError("An error occurred.");
                WriteError("Message: " + ex.Message);
            }

            WriteInfo("Press any key to exit.");
            Console.ReadKey();
        }

   

        static void WriteInfo(string text)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(text);
        }

        static void WriteWarning(string text)
        {
            Console.ForegroundColor =  ConsoleColor.Yellow;
            Console.WriteLine(text);
        }

        static void WriteError(string text)
        {
            Console.ForegroundColor =  ConsoleColor.Red;
            Console.WriteLine(text);
        }
    }
}
