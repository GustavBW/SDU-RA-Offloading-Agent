using System;
using System.Collections.Generic;
using MonitoringSubsystem;

namespace NNAgent;

public class OffloadingAgentApplication
{
    private static void Main(string[] args)
    {
        Console.WriteLine("[RA-OA] Starting Application");
        MonitoringSubsystem.SubsystemController.Init();
        //The subsystem is now running and offloadet to its own thread. You can do anything below this point without interfering.
        
        Console.WriteLine("[RA-OA] Application shutdown");
    }
}