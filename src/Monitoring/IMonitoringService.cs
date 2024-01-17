using System;
using System.Collections.Generic;

namespace Monitoring;

public interface IMonitoringService
{
    IList<int> GetServiceNodePorts(String serviceName);
    void ScaleDeployment(string deploymentName, int replicas);
    IList<String> GetDeployments();
    string GetDeploymentStatus(string deploymentName);
    int GetReplicas(string deploymentName);
    IList<string> GetNodeNames();

    IDictionary<string, string> GetLabelsOfNode(string nodeName);
    float GetNodeCpuUsage(string nodeName);
    float GetPercentageOfCpuUsage(string nodeName);
    float GetTotalCpu(string nodeName);
    float GetCurrentWatt();
}