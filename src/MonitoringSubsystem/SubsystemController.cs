using System;
using System.Threading;
using Monitoring;

namespace MonitoringSubsystem;

public class SubsystemController
{
    private static Thread _subsystemThread;
    private static SubsystemController _instance;
    private static volatile bool ShouldRun = true;

    /**
     * Spawns a new thread, and initializes a subsystem on this thread. <br/>
     * pollRate, uint32, default = 1000, how many times a second to try and update all monitored values. 
     */
    public static void Init(Action<Exception> onIrrevocableError, uint pollRate = 1000, uint maxConnectionAttempts = 10)
    {
        if (_instance != null && _subsystemThread != null)
        {
            Console.Error.WriteLine("[MSC] Ignored foolish attempt to re-initialize already running subsystem. pff.");
            return;
        }
        Console.WriteLine("[MSC] Initizalizing ");
        Console.WriteLine("[MSC] Using Monitoring version: " + Monitoring.ModuleInfo.VERSION);

        _subsystemThread = new Thread(() => new SubsystemController(pollRate,onIrrevocableError,maxConnectionAttempts).Start());
        _subsystemThread.Start();
    }

    public static void Stop()
    {
        ShouldRun = false;
        _subsystemThread.Join();
    }

    private void Start()
    {
        bool connectionEstablished = true;
        bool completeConnectionFailure = false;
        IMonitoringService monitoringService;
        int secondsToSleepBeforeTryingAgain = 2;
        uint failedAttempts = 0;
        do {
            try
            {
                monitoringService = new KubernetesMonitoring();
            }
            catch (Exception e)
            {
                if (failedAttempts > _maxConnectionAttempts)
                {
                    Console.Error.WriteLine("[MSC] Connection failed. Aborting.");
                    _onIrrevocableError(e);
                    completeConnectionFailure = true;
                }
                else
                {
                    connectionEstablished = false;
                    failedAttempts++;
                    Console.Error.WriteLine("[MSC] Unable to connect. Trying again in " + secondsToSleepBeforeTryingAgain + " seconds...");
                    Thread.Sleep(secondsToSleepBeforeTryingAgain * 1000);
                    secondsToSleepBeforeTryingAgain = (int) Math.Pow(secondsToSleepBeforeTryingAgain, 1.5);
                }
            }
        } while (!connectionEstablished && !completeConnectionFailure);

        ShouldRun = completeConnectionFailure;
        
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
    
    private uint _pollRate;
    private readonly Action<Exception> _onIrrevocableError;
    private readonly uint _maxConnectionAttempts;
    
    private SubsystemController(uint pollRate, Action<Exception> onIrrevocableError, uint maxConnectionAttempts)
    {
        this._onIrrevocableError = onIrrevocableError;
        this._maxConnectionAttempts = maxConnectionAttempts;
        this._pollRate = pollRate;
        _instance = this;
    }
}
