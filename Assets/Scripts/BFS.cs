using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BFS
{
    public static Dictionary<T_Node, T_Node> GetParentMap<T_Node>(T_Node start, Graph<T_Node> graph) where T_Node : class
    {
        Dictionary<T_Node, T_Node> parentMap = new Dictionary<T_Node, T_Node>();

        if (!graph.edges.ContainsKey(start)) return parentMap;

        Queue<T_Node> queue = new Queue<T_Node>();
        queue.Enqueue(start);
        parentMap.Add(start, null);

        while (queue.Count > 0)
        {
            T_Node current = queue.Dequeue();

            foreach (T_Node n in graph.edges[current])
            {
                if (!parentMap.ContainsKey(n))
                {
                    parentMap.Add(n, current);
                    queue.Enqueue(n);
                }
            }
        }

        return parentMap;
    }

    public static HashSet<T_Node> GetConnectedNodes<T_Node>(T_Node start, Graph<T_Node> graph) where T_Node : class
    {
        var retVal = new HashSet<T_Node>();

        var parentMap = GetParentMap(start, graph);

        foreach(var node in parentMap.Keys)
        {
            retVal.Add(node);
        }
        retVal.Add(start);

        return retVal;
    }

    public static List<HashSet<T_Node>> GetNodeDepths<T_Node>(Dictionary<T_Node, T_Node> parentMap) where T_Node : class
    {
        List<HashSet<T_Node>> nodeDepths = new List<HashSet<T_Node>>();

        foreach (T_Node child in parentMap.Keys)
        {
            Stack<T_Node> nodeStack = new Stack<T_Node>();
            T_Node tempNode = child;
            while (tempNode != null)
            {
                nodeStack.Push(tempNode);
                tempNode = parentMap[tempNode];
            }
            for (int i = 0; nodeStack.Count != 0; i++)
            {
                if (nodeDepths.Count <= i)
                {
                    nodeDepths.Add(new HashSet<T_Node>());
                }
                nodeDepths[i].Add(nodeStack.Pop());
            }
        }
        return nodeDepths;
    }

    public static List<HashSet<T_Node>> GetNodeDepthsTarget<T_Node>(Dictionary<T_Node, T_Node> parentMap, Graph<T_Node> graph, T_Node target) where T_Node : class
    {
        List<HashSet<T_Node>> nodeDepthsTarget = new List<HashSet<T_Node>>();

        List<HashSet<T_Node>> nodeDepths = GetNodeDepths(parentMap);
        List<T_Node> shortestPath = ShortestPath(parentMap, target);

        if (shortestPath == null) return null;

        int maxLen = shortestPath.Count;

        for (int i = 0; i < maxLen; i++) nodeDepthsTarget.Add(new HashSet<T_Node>());

        // add end node
        nodeDepthsTarget[maxLen - 1].Add(target);

        // go from target and follow path to nodes that are reachable at optimal pace from current node
        for (int i = maxLen - 1; i > 0; i--)
        {
            foreach (T_Node farNode in nodeDepthsTarget[i])
            {
                if (!graph.edges.ContainsKey(farNode)) continue;
                foreach (T_Node closeNode in nodeDepths[i - 1])
                {
                    if (graph.edges[farNode].Contains(closeNode))
                    {
                        nodeDepthsTarget[i - 1].Add(closeNode);
                    }
                }
            }
        }
        return nodeDepthsTarget;
    }

    public static List<List<T_Node>> GetAllOptPaths<T_Node>(Dictionary<T_Node, T_Node> parentMap, Graph<T_Node> graph, T_Node target) where T_Node : class
    {
        List<List<T_Node>> allPaths = new List<List<T_Node>>();

        List<HashSet<T_Node>> nodeDepthsTarget = GetNodeDepthsTarget(parentMap, graph, target);
        if (nodeDepthsTarget == null || nodeDepthsTarget.Count == 0) return allPaths; // invalid inputs

        int totDepth = nodeDepthsTarget.Count;
        // traverse tree and add to lists at leaf nodes
        void RecurseAddList(List<T_Node> listSoFar, int depth)
        {
            if (depth == totDepth) // base case at overshooting last node
            {
                allPaths.Add(listSoFar);
            }
            else
            {
                if (!graph.edges.ContainsKey(listSoFar[depth - 1])) return;
                var edgesFromNode = graph.edges[listSoFar[depth - 1]];
                foreach (T_Node node in nodeDepthsTarget[depth])
                {
                    if (edgesFromNode.Contains(node))
                    {
                        List<T_Node> listCopy = new List<T_Node>(listSoFar) { node };
                        RecurseAddList(listCopy, depth + 1);
                    }
                }
            }
        }

        // add fresh list of starting nodes
        foreach (T_Node node in nodeDepthsTarget[0])
        {
            RecurseAddList(new List<T_Node>() { node }, 1);
        }
        return allPaths;
    }

    public static int GetNumOptPaths<T_Node>(Dictionary<T_Node, T_Node> parentMap, Graph<T_Node> graph, T_Node target) where T_Node : class
    {
        List<HashSet<T_Node>> nodeDepthsTarget = GetNodeDepthsTarget(parentMap, graph, target);
        if (nodeDepthsTarget == null || nodeDepthsTarget.Count == 0) return -1; // invalid inputs

        Dictionary<T_Node, int> pathsToNode = new Dictionary<T_Node, int>();
        // 1 path to all starting nodes
        foreach (T_Node node in nodeDepthsTarget[0])
        {
            pathsToNode.Add(node, 1);
        }

        // dynamic programming set all further path numbers
        // iterate all pairs of depths
        for (int i = 1; i < nodeDepthsTarget.Count; i++)
        {
            // iterate all further nodes (closer to target)
            foreach (T_Node farNode in nodeDepthsTarget[i])
            {
                int paths = 0;
                // iterate all closer nodes (closer to start)
                foreach (T_Node closeNode in nodeDepthsTarget[i - 1])
                {
                    // add paths of all closer nodes
                    if (graph.edges.ContainsKey(farNode) && graph.edges[farNode].Contains(closeNode))
                    {
                        paths += pathsToNode[closeNode];
                    }
                }
                pathsToNode.Add(farNode, paths);
            }
        }
        return pathsToNode[target];
    }

    public static List<T_Node> ShortestPath<T_Node>(Dictionary<T_Node, T_Node> parentMap, T_Node target) where T_Node : class =>
        ShortestPath(parentMap, new List<T_Node>() { target });

    public static List<T_Node> ShortestPath<T_Node>(Dictionary<T_Node, T_Node> parentMap, List<T_Node> targets) where T_Node : class
    {
        List<T_Node> retVal = null;
        foreach (T_Node target in targets)
        {
            if (parentMap.ContainsKey(target))
            {
                List<T_Node> currentList = new List<T_Node>();
                T_Node next = target;
                while (next != null)
                {
                    currentList.Add(next);
                    next = parentMap[next];
                }
                if (retVal == null || currentList.Count < retVal.Count)
                {
                    retVal = currentList;
                }
            }
        }
        if (retVal == null) return null;
        retVal.Reverse();
        return retVal;
    }


    public static int AccessableSize<T_Node>(Dictionary<T_Node, T_Node> parentMap) where T_Node : class
    {
        int retVal = 0;
        foreach (T_Node n in parentMap.Values)
        {
            retVal++;
        }
        return retVal;
    }


    public abstract class Node<T> : IEquatable<T>
    {
        public abstract bool Equals(T other);
        public abstract override int GetHashCode();
    }

    public abstract class Graph<T_Node>
    {
        public abstract Dictionary<T_Node, HashSet<T_Node>> edges { get; set; }
        // public List<T_Node> starts, goals;
    }
}
