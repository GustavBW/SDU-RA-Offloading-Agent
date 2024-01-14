using System;
using System.Threading;
using Monitoring;

namespace MonitoringSubsystem;

public class SubsystemController
{
    private static Thread subsystemThread;
    private static SubsystemController instance;
    
    /*
     * Spawns a new thread, and initializes a subsystem on this thread.
     * Don't call this method more than once, since the instance returned by GetInstance() will be replaced each time. 
     */
    public static void Init()
    {
        Console.WriteLine("[MSC] Initizalizing ");
        Console.WriteLine("[MSC] Using Monitoring version: " + Monitoring.ModuleInfo.VERSION);

        subsystemThread = new Thread(() => new SubsystemController().Start());
        subsystemThread.Start();
        subsystemThread.Join();
    }

    /**
     * May return null
     */
    public static SubsystemController GetInstance()
    {
        return instance!;
    }

    private void Start()
    {
        instance = this;
        Console.WriteLine("[MSC] SubsystemThread is: " + subsystemThread + "#" + subsystemThread.GetHashCode());
        Console.WriteLine("[MSC] Subsystem instance is: " + instance + "#" + instance.GetHashCode());
        
        
        
        
    }
}