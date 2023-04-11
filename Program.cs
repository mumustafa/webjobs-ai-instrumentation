using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        TelemetryConfiguration configuration = TelemetryConfiguration.CreateDefault();
        configuration.ConnectionString = "<Your Application Insights Connection String>";
        TelemetryClient telemetry = new TelemetryClient(configuration);
        
        /// TelemetryProcessorChainBuilder is a class that allows you to build a chain of telemetry processors that can modify or filter telemetry data before it is sent to the Application Insights service.
        /// Below isCustom telemetry processor named MyTelemetryProcessor is being added to the chain using the Use method. 
        /// This processor will be invoked before any other processors in the chain. The next parameter represents the next processor in the chain,
        /// which the custom processor can invoke to pass the telemetry data along the chain. 
        /// Finally, the Build method is called to create an instance of the telemetry processor chain that can be used to process telemetry data.
        /// Refer to the Class MyTelemetryProcessor()

        ///////////////////////////////////////////////////////////////////////

        var perfCollectorModule = new PerformanceCollectorModule();
        //var webjobname = Environment.GetEnvironmentVariable("WEBJOBS_NAME");

        
        perfCollectorModule.Initialize(configuration);


        //////////////// This part is similar to the above but will add the webjob name ////////////////////////////////////
        //// Check the Class WebJobNameTelemetryProcessor() ////////////////////
        configuration.TelemetryProcessorChainBuilder
            .Use((next) => new WebJobNameTelemetryProcessor(next))
            .Build();

        ///////////////////////////////////////////////////////////////////////

        
        // This Example poulates with the default customDimensions, WebJob Name is not included
        perfCollectorModule.Counters.Add(new PerformanceCounterCollectionRequest(
        @"\Process(??APP_WIN32_PROC??)\IO Data Bytes/sec", "IO Data Bytes/sec"));


        // The below section is Custom Metrics/Events example

        telemetry.Context.Cloud.RoleInstance = Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID");
        telemetry.Context.Cloud.RoleName = Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME");

        telemetry.Context.GlobalProperties["InstanceName"] = telemetry.Context.Cloud.RoleInstance;
        telemetry.Context.GlobalProperties["WebJobName"] = telemetry.Context.Cloud.RoleName + "/" + Environment.GetEnvironmentVariable("WEBJOBS_NAME");
        var list = new List<byte[]>();

        while (true)
        {
            try
            {
                telemetry.TrackEvent("Count started");
                List<int> numbers = new List<int>();
                for (int i = 1; i <= 200; i++)
                {
                    numbers.Add(i);
                    list.Add(new byte[1024]);
                    
                }
                Parallel.ForEach(numbers, (num) =>
                {
                    Console.WriteLine(num);
                    telemetry.TrackMetric("Number", num);
                    Thread.Sleep(1000);
                });

                telemetry.TrackEvent("Count completed");

                var process = Process.GetCurrentProcess();
                var userTime = process.UserProcessorTime;
                // Example of CPU Usage, this is just an example, please use your OWN calculation
                var cpuUsage = userTime.Ticks > 0 ? (float)userTime.TotalMilliseconds / (Environment.ProcessorCount * TimeSpan.TicksPerMillisecond) / (float)process.TotalProcessorTime.Ticks * 100.0f : 0;
                // The below will measure total processer time instead of userTime, this is just an example, please use your OWN calculation
                var cpuProcUsage = process.TotalProcessorTime.Ticks > 0 ? (float)process.TotalProcessorTime.TotalMilliseconds / (Environment.ProcessorCount * TimeSpan.TicksPerMillisecond) / (float)process.TotalProcessorTime.Ticks * 100.0f : 0;
                var memUsage = process.WorkingSet64 / (1024 * 1024);
                
                // This where the metric is tracked as a custom metric

                telemetry.TrackMetric("CPU Usage", cpuUsage);
                telemetry.TrackMetric("CPU Proc Usage", cpuProcUsage);
                telemetry.TrackMetric("Memory Usage", memUsage);

                Thread.Sleep(300000); // 300k milliseconds, which is equivalent to 5 minutes
                //Thread.Sleep(600000);
            }
            catch (Exception ex)
            {
                telemetry.TrackException(ex);
                throw;
            }
        }

    }
   
}

//// WebJobNameTelemetryProcessor Class ///////////
/// The ITelemetryProcessor interface is part of the Azure Application Insights SDK and defines a contract for processing telemetry data before it is sent to the Application Insights service.
/// Classes that implement this interface can modify, filter, or perform any other processing on the telemetry data before it is sent to the service. 
public class WebJobNameTelemetryProcessor : ITelemetryProcessor
{
    private readonly ITelemetryProcessor _next;

    public WebJobNameTelemetryProcessor(ITelemetryProcessor next)
    {
        _next = next;
    }

    public void Process(ITelemetry telemetry)
    {
        // Set WebJob name as custom property
        telemetry.Context.GlobalProperties["WebJobName"] = telemetry.Context.Cloud.RoleName + "/" + Environment.GetEnvironmentVariable("WEBJOBS_NAME");
        _next.Process(telemetry);
    }
}

