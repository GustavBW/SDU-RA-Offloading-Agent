using System;
using System.Collections.Generic;

namespace OffloadingAgent;

public class OffloadingAgentApplication
{

    private static readonly List<NamedVoidAction> onShutdownDo = new List<NamedVoidAction>();
    
    public static void addShutdownHook(NamedVoidAction func)
    {
        onShutdownDo.Add(func);
    }
    
    
    static void Main(string[] args)
    {
        Console.WriteLine("RA-OA: Starting Application");
        addShutdownHook(new NamedVoidAction("Func1", () => Console.WriteLine("I'm a hooker")));
        //code here
        
        foreach (NamedVoidAction action in onShutdownDo)
        {
            Console.WriteLine("Executing: " + action.Name);
            action.Action();
        }
        Console.WriteLine("RA-OA: Application shutdown");
    }
}