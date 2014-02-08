using System;
using Microsoft.SqlServer.Management.Smo;
using System.Threading;
using Microsoft.SqlServer.Management.Smo.Agent;
using sqlservermanagementobjects.Common;
using System.Data;
namespace sqlservermanagementobjects
{
    class Program
    {
        const string _server = "(local)";       // this will only work if you are running a default local sql instance, otherwise substitute your given server here.
        const string _job = "MyAwesomeJob";

        static void Main(string[] args)
        {
            
            try
            {
                WriteLine("Connecting to " + _server + " instance of SQL Server 2008...", ConsoleColor.Green);

                Server server = new Server(_server);

                if (server.ConnectionContext.IsOpen)
                {
                    WriteLine("Oh no, something weird happened! The Server object shouldnt be opened at this point!", ConsoleColor.Red);
                    return;
                }
                WriteLine("Server object is active (but not connected)!",  ConsoleColor.Green);

                WriteLine("Retrieving '" + _job + "' SQL Job on the local SQL Agent.", ConsoleColor.Green);
                var job = server.JobServer.Jobs[_job];

                WriteLine("Job object is active.", ConsoleColor.Green);
                job.Start();

                Thread.Sleep(1000); // I realise this isnt ideal, but there can be a delay between the 
                                    //.Start() and the status of the Job updating correctly, indicating the job is being executed.

                job.Refresh();

                while (true)
                {

                    if (job.CurrentRunStatus != JobExecutionStatus.Executing)
                    {
                        if (job.LastRunOutcome == CompletionResult.Succeeded)
                            WriteLine("Hooray! Job Successful", ConsoleColor.Cyan);
                        else
                        {
                            WriteLine("Oh Noes! Job failed!", ConsoleColor.Red);
                            WriteLine("Job failed with message: " + job.GetLastFailedMessageFromJob(), ConsoleColor.Red);
                        }
                           
                        break;
                    }
                    job.Refresh();      // You must call refresh each iteration, as the property doesnt update automatically.

                    WriteLine("Waiting...", ConsoleColor.Yellow);
                    Thread.Sleep(1000);
                }
            }
            catch(Exception ex)
            {
                WriteLine("An error occurred.", ConsoleColor.Red);
                WriteLine("Message: " + ex.Message, ConsoleColor.Red);
            }

            WriteLine("Press any key to exit.", ConsoleColor.Cyan);
            Console.ReadKey();
        }

        static void WriteLine(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
        }

    }
}
