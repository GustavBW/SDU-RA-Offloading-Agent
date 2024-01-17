using System;
using System.Collections.Generic;
using System.Threading;
using MonitoringSubsystem;

namespace NNAgent;

public class OffloadingAgentApplication
{
    private static bool _shouldRun = true;

    private static Action<Exception> _onIrrevocableError = e =>
    {
        Console.Error.WriteLine("[RA-OA] System Failure: " + e.Message);
        _shouldRun = false;
    };
    private static void Main(string[] args)
    {
        Console.WriteLine("[RA-OA] Starting Application");
        MonitoringSubsystem.SubsystemController.Init(_onIrrevocableError, 1,3);
        
        while (_shouldRun)
        {
            Console.WriteLine("[RA-OA] ..is feeling sleepy");
            Thread.Sleep(1000);
        }
        
        
        MonitoringSubsystem.SubsystemController.Stop();
        Console.WriteLine("[RA-OA] Application shutdown");
    }
}