using System;
using System.Collections.Generic;
using MonitoringSubsystem;

namespace NNAgent;

public class OffloadingAgentApplication
{
    private static void Main(string[] args)
    {
        Console.WriteLine("[RA-OA] Starting Application");
        Console.WriteLine("Initializing Monitoring Subsystem version: " + MonitoringSubsystem.ModuleInfo.VERSION);
        MonitoringSubsystem.SubsystemController.Init();
        
        Console.WriteLine("[RA-OA] Application shutdown");
    }
}