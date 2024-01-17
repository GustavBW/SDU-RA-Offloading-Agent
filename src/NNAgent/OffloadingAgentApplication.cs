using System;
using System.Collections.Generic;
using System.Threading;
using Common.Models;
using MonitoringDaemon;

namespace NNAgent;

public class OffloadingAgentApplication
{
    private static bool _shouldRun = true;
    
    private static void Main(string[] args)
    {
        Console.WriteLine("[RA-OA] Starting Application");
        Tuple<StateCache, Exception> initAttempt = SubsystemController.Init( 1,3);

        if (initAttempt.Item2 != null)
        {
            _shouldRun = false;
            Console.Error.WriteLine("[RA-OA] Failed to initialize daemon: " + initAttempt.Item2.Message);
        }
        
        while (_shouldRun)
        {
            Thread.Sleep(1000);
        }
        
        
        MonitoringDaemon.SubsystemController.Stop();
        Console.WriteLine("[RA-OA] Application shutdown");
    }
}