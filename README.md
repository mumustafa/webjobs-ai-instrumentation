# WebJobs Instrumentation example
### This is an example WebJob that have application inisghts instrutmentation using Application Insights SDK
### In this example You have the default counters in the performanceCounters table and it will log the webjob name as well.
### Custom metrics are also capturing the webjob name and example metrics
## Important resources:
1. Microsoft Docs - Application Insights:  https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview
2. TelemetryProcessorChainBuilder class reference:  https://docs.microsoft.com/en-us/dotnet/api/microsoft.applicationinsights.extensibility.telemetryconfiguration.telemetryprocessorchainbuilder?view=azure-dotnet
3. Custom Telemetry Processors in Application Insights:  https://docs.microsoft.com/en-us/azure/azure-monitor/app/custom-operations-tracking#telemetry-processors
4. Application Insights SDK for .NET:  https://github.com/microsoft/ApplicationInsights-dotnet 

## Metrics Sources
1. TotalProcessorTime Property:  https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process.totalprocessortime?view=net-5.0
2. UserProcessorTime Property:  https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process.userprocessortime?view=net-5.0
3.  WorkingSet64 Property:  https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process.workingset64?view=net-5.0
4. Parallel.ForEach() Method:  https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.parallel.foreach?view=net-5.0
5. Application Insights Metrics:  https://docs.microsoft.com/en-us/azure/azure-monitor/app/api-custom-events-metrics#metrics
