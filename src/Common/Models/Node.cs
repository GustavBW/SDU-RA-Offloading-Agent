using MonitoringSubsystem;

namespace Common.Models;

public struct Node
{
    public readonly string Identifier;
    public ThreadSafeValue<float> PowerPerCompletion;
    public ThreadSafeValue<uint> TimePerCompletion;
    public ThreadSafeValue<float> CpuPerCompletion;
}