using System;
using System.Threading;
using Common.Models;
using Monitoring;

namespace MonitoringDaemon;

public class SubsystemController
{
    private static Thread _subsystemThread;
    private static SubsystemController _instance;
    private static volatile bool ShouldRun = true;

    /**
     * Spawns a new thread, and initializes a subsystem on this thread. <br/>
     * pollRate, uint32, default = 1000, how many times a second to try and update all monitored values. 
     */
    public static Tuple<StateCache,Exception> Init(uint pollRate = 1000, uint maxConnectionAttempts = 10)
    {
        if (_instance != null && _subsystemThread != null)
        {
            Console.Error.WriteLine("[MSC] Ignored attempt to re-initialize already running subsystem.");
            return null;
        }
        Console.WriteLine("[MSC] Connecting & Initializing ");
        Console.WriteLine("[MSC] Using Monitoring version: " + Monitoring.ModuleInfo.VERSION);

        Tuple<IMonitoringService,Exception> res = MonitoringConnector.ConnectAndGetCache(maxConnectionAttempts);
        if (res.Item2 != null)
        {
            return new(null, res.Item2);
        }

        StateCache cache = new StateCache(res.Item1.GetNodeNames());
        
        _subsystemThread = new Thread(() => new SubsystemController(pollRate, res.Item1, cache).Start());
        _subsystemThread.Start();

        return new(cache, null);
    }

    public static void Stop()
    {
        ShouldRun = false;
        if (_subsystemThread is { IsAlive: true })
        {
            _subsystemThread.Join();
        }
    }

    private void Start()
    {
        
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
        return _instance;
    }
    
    private readonly uint _pollRate;
    private readonly IMonitoringService _monitoringService;
    private readonly StateCache _cache;
    
    private SubsystemController(uint pollRate, IMonitoringService monitoringService, StateCache cache)
    {
        _pollRate = pollRate;
        _monitoringService = monitoringService;
        _cache = cache;
        _instance = this;
    }
}
