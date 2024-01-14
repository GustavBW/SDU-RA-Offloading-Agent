using System;
using System.Collections.Generic;

namespace NNAgent;

public class OffloadingAgentApplication
{
    private static readonly List<NamedVoidAction> onShutdownDo = new();

    public static void addShutdownHook(NamedVoidAction func)
    {
        onShutdownDo.Add(func);
    }


    private static void Main(string[] args)
    {
        Console.WriteLine("RA-OA: Starting Application");
        addShutdownHook(new NamedVoidAction("Func1", () => Console.WriteLine("I'm hooked")));
        //code here

        foreach (var action in onShutdownDo)
        {
            Console.WriteLine("Executing: " + action.Name);
            action.Action();
        }

        Console.WriteLine("RA-OA: Application shutdown");
    }
}