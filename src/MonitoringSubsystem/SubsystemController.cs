using System;
using System.Threading;
using Monitoring;

namespace MonitoringSubsystem;

public class SubsystemController
{
    private static Thread _subsystemThread;
    private static SubsystemController _instance;
    private static volatile bool ShouldRun = true;
    private uint _pollRate;
    
    /**
     * Spawns a new thread, and initializes a subsystem on this thread. <br/>
     * pollRate, uint32, default = 1000, how many times a second to try and update all monitored values. 
     */
    public static void Init(uint pollRate = 1000)
    {
        if (_instance != null && _subsystemThread != null)
        {
            Console.Error.WriteLine("[MSC] Ignored foolish attempt to re-initialize already running subsystem. pff.");
            return;
        }
        Console.WriteLine("[MSC] Initizalizing ");
        Console.WriteLine("[MSC] Using Monitoring version: " + Monitoring.ModuleInfo.VERSION);

        _subsystemThread = new Thread(() => new SubsystemController(pollRate).Start());
        _subsystemThread.Start();
    }

    public static void Stop()
    {
        ShouldRun = false;
        _subsystemThread.Join();
    }

    private void Start()
    {
        Console.WriteLine("[MSC] SubsystemThread is: " + _subsystemThread + "#" + _subsystemThread.GetHashCode());
        Console.WriteLine("[MSC] Subsystem instance is: " + _instance + "#" + _instance.GetHashCode());

        while (ShouldRun)
        {
            long msLastUpdateRisingEdge = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            
            //Stuff here

  
            Sleep(msLastUpdateRisingEdge, _pollRate);
        }
        
        Console.WriteLine("[MSC] Subsystem shutdown");
    }

    private void Sleep(long msLastUpdateRisingEdge, uint pollRate)
    {
        //msPerRefresh - deltaTime 
        long msToSleep = (1000 / pollRate) - (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - msLastUpdateRisingEdge); 
        if (msToSleep > 0)
        {
            Thread.Sleep((int)msToSleep);
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
        _instance = this;
    }
}