using System;
using System.Collections.Concurrent;
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
    public static Tuple<StateCache,Exception> Init(ConcurrentBag<Action> queueOnMain, DubiousFuncExecutor<IMonitoringService> monitoringProvider, uint pollRate = 1000, uint maxConnectionAttempts = 10)
    {
        if (_instance != null && _subsystemThread != null)
        {
            Console.Error.WriteLine("[MD] Ignored attempt to re-initialize already running subsystem.");
            return new(null,null);
        }
        Console.WriteLine("[MD] Connecting & Initializing ");

        Tuple<IMonitoringService,Exception> res = monitoringProvider.TryExec(maxConnectionAttempts);
        if (res.Item2 != null)
        {
            return new(null, res.Item2);
        }

        StateCache cache = new StateCache(res.Item1.GetNodeNames());
        
        _subsystemThread = new Thread(() => new SubsystemController(queueOnMain, pollRate, res.Item1, cache).Start());
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
        
        Console.WriteLine("[MD] Subsystem shutdown");
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
    private readonly ConcurrentBag<Action> _queueOnMain;
    
    private SubsystemController(ConcurrentBag<Action> queueOnMain, uint pollRate, IMonitoringService monitoringService, StateCache cache)
    {
        _pollRate = pollRate;
        _monitoringService = monitoringService;
        _cache = cache;
        _instance = this;
        _queueOnMain = queueOnMain;
    }
}
