using System;
using System.Threading;
using Monitoring;
using Common.Models;

namespace MonitoringDaemon;

public class MonitoringConnector
{
    public static Tuple<IMonitoringService, Exception> ConnectAndGetCache(uint maxConnectionAttempts)
    {
        IMonitoringService monitoringService = null;
        
        bool connectionEstablished = true;
        bool completeConnectionFailure = false;
        int secondsToSleepBeforeTryingAgain = 2;
        uint failedAttempts = 0;
        Exception latestError = null;
        
        do {
            try
            {
                monitoringService = new KubernetesMonitoring();
            }
            catch (Exception e)
            {
                latestError = e;
                if (failedAttempts > maxConnectionAttempts)
                {
                    Console.Error.WriteLine("[MC] Connection failed. Aborting.");
                    completeConnectionFailure = true;
                }
                else
                {
                    connectionEstablished = false;
                    failedAttempts++;
                    Console.Error.WriteLine("[MC] Unable to connect. Trying again in " + secondsToSleepBeforeTryingAgain + " seconds...");
                    Thread.Sleep(secondsToSleepBeforeTryingAgain * 1000);
                    secondsToSleepBeforeTryingAgain = (int) Math.Pow(secondsToSleepBeforeTryingAgain, 1.5);
                }
            }
        } while (!connectionEstablished && !completeConnectionFailure);

        if (monitoringService != null)
        {
            return new(monitoringService, null);
        }

        return new(null, latestError);
    }
}