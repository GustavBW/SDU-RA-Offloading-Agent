using System;
using System.Collections.Generic;
using System.Linq;
using k8s;
using k8s.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Monitoring;

public class KubernetesMonitoring : IMonitoringService
{
    private readonly k8s.Kubernetes _client;
    private readonly IList<V1Service> _services;
    private readonly IList<V1Deployment> _deployments;
    private readonly HMCDeviceWrapper _hmcDeviceWrapper;

    public KubernetesMonitoring()
    {
        KubernetesClientConfiguration _config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
        _client = new k8s.Kubernetes(_config);
        _services = _client.ListServiceForAllNamespaces().Items;
        _deployments = _client.ListDeploymentForAllNamespaces().Items;
        //_hmcDeviceWrapper = new HMCDeviceWrapper("TCPIP::192.168.56.101::5025::SOCKET");
    }

    public IList<int> GetServiceNodePorts(String serviceName)
    {
        var service = _services.First(s => s.Metadata.Name == serviceName);
        var ports = service.Spec.Ports;
        return (from port in ports select port.NodePort.Value).ToList();
    }

    public void ScaleDeployment(string deploymentName, int replicas)
    {
        var deployment = _deployments.First(d => d.Metadata.Name == deploymentName);
        deployment.Spec.Replicas = replicas;
    }

    public IList<string> GetDeployments()
    {
        return _client.ListDeploymentForAllNamespaces().Items.Select(d => d.Metadata.Name).ToList();
    }

    public string GetDeploymentStatus(string deploymentName)
    {
        var deployment = _deployments.First(d => d.Metadata.Name == deploymentName);
        return deployment.Status.AvailableReplicas == deployment.Spec.Replicas ? "Ready" : "Not Updated";
    }

    public int GetReplicas(string deploymentName)
    {
        var deployment = _deployments.First(d => d.Metadata.Name == deploymentName);
        return deployment.Spec.Replicas.Value;
    }

    public IList<string> GetNodeNames()
    {
        return (from node in _client.ListNode().Items select node.Metadata.Name).ToList();
    }

    public IDictionary<string, string> GetLabelsOfNode(string nodeName)
    {
        var node = _client.ListNode().Items.First(n => n.Metadata.Name == nodeName);
        return node.Metadata.Labels;
    }

    public float GetNodeCpuUsage(string nodeName)
    {
        var response = _client
            .CustomObjects
            .ListClusterCustomObject("metrics.k8s.io", "v1beta1", "nodes");

        var nodes = JsonConvert.DeserializeObject<JObject>(response.ToString());


        foreach (var item in nodes["items"])
        {
            var metadata = item["metadata"];
            if (metadata == null || metadata["name"].ToString() != nodeName) continue;
            var usage = item["usage"];
            if (usage == null || usage["cpu"] == null) continue;
            var cpuUsage = NanoToWhole(usage["cpu"].ToString());
            // _performanceLogger.Log($"Node {nodeName} CPU usage: {cpuUsage}");
            return cpuUsage;
        }

        throw new KeyNotFoundException($"Node {nodeName} not found");
    }

    public float GetPercentageOfCpuUsage(string nodeName)
    {
        var response = _client
            .CustomObjects
            .ListClusterCustomObject("metrics.k8s.io", "v1beta1", "nodes");

        var nodes = JsonConvert.DeserializeObject<JObject>(response.ToString());

        foreach (var item in nodes["items"])
        {
            var metadata = item["metadata"];
            if (metadata == null || metadata["name"].ToString() != nodeName) continue;
            var usage = item["usage"];
            if (usage == null || usage["cpu"] == null) continue;
            var cpuUsage = NanoToWhole(usage["cpu"].ToString());
            var capacity = _client.ListNode().Items.First(n => n.Metadata.Name == nodeName).Status.Capacity["cpu"]; 
            var cpuCapacity = Int32.Parse(capacity.Value);
            // _performanceLogger.Log($"Node {nodeName} CPU usage: {cpuUsage}");
            return cpuUsage / cpuCapacity * 100;
        }

        throw new KeyNotFoundException($"Node {nodeName} not found");
    }

    public float GetTotalCpu(string nodeName)
    {
        var capacity = _client.ListNode().Items.First(n => n.Metadata.Name == nodeName).Status.Capacity["cpu"]; 
        var cpuCapacity = Int32.Parse(capacity.Value);
        return cpuCapacity;
    }

    public float GetCurrentWatt()
    {
        return _hmcDeviceWrapper.GetPower();
    }

    private float NanoToWhole(string nano)
    {
        return float.Parse(nano.Substring(0, nano.Length - 1)) / 1_000_000_000;
    }
}