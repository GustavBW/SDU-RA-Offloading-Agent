using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common.Models;
using Monitoring;
using MonitoringDaemon;
using MonitoringDaemon.MonitoringSubsystem.Tests;
using MonitoringSubsystem;

namespace NNAgent;

public class OffloadingAgentApplication
{
    private static bool _shouldRun = true;
    /**
     * Add an action to this queue to have it be handled by the main thread (blocking NN-Agent). <br/>
     * If the action throws and exception, it is treated as a system error, aka not handled.
     */
    private static ConcurrentBag<Action> _queueOnMain = new(); 
    
    private static void Main(string[] args)
    {
        Console.WriteLine("[RA-OA] Starting Application");
        Tuple<StateCache, Exception> initAttempt = SubsystemController.Init(_queueOnMain,new DubiousFuncExecutor<IMonitoringService>(() => new KubernetesMonitoring()),1, 3);

        if (initAttempt.Item2 != null)
        {
            _shouldRun = false;
            Console.Error.WriteLine("[RA-OA] Failed to initialize daemon: " + initAttempt.Item2.Message);
        }
        
        while (_shouldRun)
        {
            Thread.Sleep(1000);
            // Process all items in the bag
            while (_queueOnMain.TryTake(out var action))
            {
                action();
            }
        }
        
        
        MonitoringDaemon.SubsystemController.Stop();
        Console.WriteLine("[RA-OA] Application shutdown");
    }
}