using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Legacy;

namespace Monitoring.Kubernetes.Tests;

using NUnit.Framework;

[TestFixture]
public class TypeSafeOperations
{
    private KubernetesMonitoring _monitoring;
    private readonly String[] _expectedNodeNames = { "joakim" };
    private readonly String[] _expectedDeployments = { "edge", "cloud", "small" };
    [SetUp]
    public void Setup()
    {
        _monitoring = new KubernetesMonitoring();
        
    }

    [Test]
    public void can_get_deployments_edge_cloud_small()
    {
        var deployments = _monitoring.GetDeployments();

        CollectionAssert.IsSubsetOf(_expectedDeployments, deployments, "Expected deployments not found");
    }

    [Test]
    public void can_get_correct_nodeports_for_edge_cloud_small()
    {
        var nodePorts = _expectedDeployments.SelectMany(s => _monitoring.GetServiceNodePorts(s)).ToList();
        //HARD CODED CHANGE
        var expectedPorts = new[] { 30114, 31604, 32449 };


        CollectionAssert.IsSubsetOf(expectedPorts, nodePorts, "Expected nodeports not found");
    }

    [Test]
    public void can_scale_deployment_to_2()
    {
        var deploymentName = _expectedDeployments[0];
        var replicas = 2;

        _monitoring.ScaleDeployment(deploymentName, replicas);
        var actual = _monitoring.GetReplicas(deploymentName);

        Assert.That(replicas, Is.EqualTo(actual));
    }

    [Test]
    public void can_get_node_names()
    {
        var nodeNames = _monitoring.GetNodeNames();
        //HARD CODED CHANGE

        CollectionAssert.IsSubsetOf(_expectedNodeNames, nodeNames, "Expected node names not found");
    }
    
    [Test]
    public void can_get_labels_of_node()
    {
        var nodeName = _expectedNodeNames[0];
        var labels = _monitoring.GetLabelsOfNode(nodeName);
        //HARD CODED CHANGE
        var expected = new Dictionary<string, string> { { "kubernetes.io/hostname", "joakim" } };

        CollectionAssert.IsSubsetOf(expected, labels, "Expected labels not found");
    }

    [Test]
    public void can_get_cpu_usage_of_node()
    {
        var nodeName = _expectedNodeNames[0];
        var cpuUsage = _monitoring.GetNodeCpuUsage(nodeName);

        Assert.That(cpuUsage, Is.GreaterThan(0));
    }
    
    [Test]
    public void can_get_percentage_of_cpu_usage_of_node()
    {
        var nodeName = _expectedNodeNames[0];
        var percentageOfCpuUsage = _monitoring.GetPercentageOfCpuUsage(nodeName);

        Assert.That(percentageOfCpuUsage, Is.GreaterThan(0));
    }
    
    [Test]
    public void can_get_total_cpu_of_node()
    {
        var nodeName = _expectedNodeNames[0];
        var totalCpu = _monitoring.GetTotalCpu(nodeName);

        //HARD CODED CHANGE
        Assert.That(totalCpu, Is.EqualTo(8));
    }
    
    [TearDown]
    public void TearDown()
    {
        _monitoring.ScaleDeployment(_expectedDeployments[0], 1);
    }
}