using MonitoringSubsystem;

namespace Common.Models;

public struct Node
{
    public readonly string Identifier;
    public readonly ThreadSafeValue<float> PowerPerCompletion = new(-1);
    public readonly ThreadSafeValue<uint> TimePerCompletion = new(0);
    public readonly ThreadSafeValue<float> CpuPerCompletion = new(-1);
    public Node(string identifier)
    {
        this.Identifier = identifier;
    }
    
    public override string ToString()
    {
        return "Node " + Identifier + "{PPP:" + PowerPerCompletion.Get() + ",TPC:" + TimePerCompletion.Get() + ",CPC:" + CpuPerCompletion + "}";
    }
}