using System.Collections;

namespace Common.Models;

public class StateCache
{
    private readonly Dictionary<string, int> _idIndexMap;
    private readonly Node[] _nodeCache;
    
    public StateCache(IList<string> nodeIdentifiers)
    {
        int numNodes = nodeIdentifiers.Count;
        _nodeCache = new Node[numNodes];
        _idIndexMap = new();
        for (int i = 0; i < numNodes; i++)
        {
            string currentId = nodeIdentifiers[i];
            _idIndexMap.Add(currentId, i);
            _nodeCache[i] = new Node(currentId);
        }
    }

    public Node[] Nodes()
    {
        return _nodeCache;
    }

    public Node GetById(string id)
    {
        return _nodeCache[_idIndexMap[id]];
    }

    
}