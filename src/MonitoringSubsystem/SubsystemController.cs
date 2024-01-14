using System;
using System.Threading;
using Monitoring;

namespace MonitoringSubsystem;

public class SubsystemController
{
    private static Thread _subsystemThread;
    private static SubsystemController _instance;
    public static volatile bool ShouldRun = true;
    private uint _pollRate;
    
    /**
     * Spawns a new thread, and initializes a subsystem on this thread.
     * Don't call this method more than once, since the instance returned by GetInstance() will be replaced each time. <br/>
     * pollRate, uint32, default = 1000, how many times a second to try and update all monitored values. 
     */
    public static void Init(uint pollRate = 1000)
    {
        if (_instance != null)
        {
            Console.Error.WriteLine("Ignored foolish attempt to re-initialize already running subsystem. pff.");
            return;
        }
        Console.WriteLine("[MSC] Initizalizing ");
        Console.WriteLine("[MSC] Using Monitoring version: " + Monitoring.ModuleInfo.VERSION);

        _subsystemThread = new Thread(() => new SubsystemController(pollRate).Start());
        _subsystemThread.Start();
    }

    private void Start()
    {
        _instance = this;
        Console.WriteLine("[MSC] SubsystemThread is: " + _subsystemThread + "#" + _subsystemThread.GetHashCode());
        Console.WriteLine("[MSC] Subsystem instance is: " + _instance + "#" + _instance.GetHashCode());
        
        while (ShouldRun)
        {
            
        }

    }
    
    /**
     * May return null
     */
    public static SubsystemController GetInstance()
    {
        return _instance!;
    }
    
    private SubsystemController(uint pollRate)
    {
        this._pollRate = pollRate;
    }
}