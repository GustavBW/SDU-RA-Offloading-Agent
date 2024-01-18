#nullable enable
using System;
using System.Threading;
using Monitoring;
using Common.Models;

namespace MonitoringDaemon;

public class DubiousFuncExecutor<T>
{

    public readonly Func<T> _func;
    
    public DubiousFuncExecutor(Func<T> func)
    {
        _func = func;
    }
    
    public Tuple<T?, Exception?> TryExec(uint maxAttempts)
    {
        T? toReturn = default(T);
        Exception? latestError = default;
        
        bool complete = false;
        double secondsToSleepBeforeTryingAgain = 2;
        uint failedAttempts = 0;
        
        do {
            try
            {
                toReturn = _func();
                complete = true;
            }
            catch (Exception? e)
            {
                latestError = e;
                if (failedAttempts > maxAttempts)
                {
                    Console.Error.WriteLine("[MC] Connection failed. Aborting.");
                    complete = true;
                }
                else
                {
                    failedAttempts++;
                    Console.Error.WriteLine("[MC] Unable to connect. Trying again in " + Math.Round(secondsToSleepBeforeTryingAgain) + " seconds...");
                    Thread.Sleep((int) secondsToSleepBeforeTryingAgain * 1000);
                    secondsToSleepBeforeTryingAgain = Math.Pow(secondsToSleepBeforeTryingAgain, 1.5);
                }
            }
        } while (!complete);

        if (toReturn != null)
        {
            return new(toReturn, null);
        }

        return new(default, latestError);
    }
}