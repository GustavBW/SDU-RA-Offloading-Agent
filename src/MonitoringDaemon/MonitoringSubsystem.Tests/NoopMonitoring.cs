using System.Collections.Generic;
using Monitoring;

namespace MonitoringDaemon.MonitoringSubsystem.Tests;

public class NoopMonitoring : IMonitoringService
{
    public IList<int> GetServiceNodePorts(string serviceName)
    {
        return new List<int>();
    }

    public void ScaleDeployment(string deploymentName, int replicas)
    {
     
    }

    public IList<string> GetDeployments()
    {
        return new List<string>();
    }

    public string GetDeploymentStatus(string deploymentName)
    {
        return "invalid";
    }

    public int GetReplicas(string deploymentName)
    {
        return -1;
    }

    public IList<string> GetNodeNames()
    {
        return new List<string>();
    }

    public IDictionary<string, string> GetLabelsOfNode(string nodeName)
    {
        return new Dictionary<string, string>();
    }

    public float GetNodeCpuUsage(string nodeName)
    {
        return -1;
    }

    public float GetPercentageOfCpuUsage(string nodeName)
    {
        return -1;
    }

    public float GetTotalCpu(string nodeName)
    {
        return -1;
    }

    public float GetCurrentWatt()
    {
        return -1;
    }
}