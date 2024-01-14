using System;
using Monitoring;

namespace MonitoringSubsystem;

public class SubsystemController
{
    public static void Init()
    {
        Console.WriteLine("[MSC] Initizalizing ");
        Console.WriteLine("[MSC] Using Monitoring version: " + Monitoring.ModuleInfo.VERSION);
    }
}