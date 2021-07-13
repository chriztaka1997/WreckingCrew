using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelAnalysis
{
    public static List<Vector2Int> PathToPlayer(Graph graph, Node start, Node end)
    {
        var parentMap = BFS.GetParentMap(start, graph);
        var shortestPath = BFS.ShortestPath(parentMap, end);

        if (shortestPath == null) return null;

        var retVal = new List<Vector2Int>();

        // start from 1 since shouldnt path to own node
        for (int i = 1; i < shortestPath.Count; i++)
        {
            retVal.Add(new Vector2Int(shortestPath[i].x, shortestPath[i].y));
        }

        return retVal;
    }


    public class Node : BFS.Node<Node>
    {
        public readonly int x;
        public readonly int y;

        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(Node other)
        {
            return x == other.x && y == other.y;
        }

        public override int GetHashCode()
        {
            return x + (y * 0x50745202);
        }

        public override string ToString()
        {
            return string.Format("x: {0}, y: {1}", x, y);
        }
    }

    public class Graph : BFS.Graph<Node>
    {
        public override Dictionary<Node, HashSet<Node>> edges { get; set; }

        public Graph(LevelTile[,] levelTiles, List<TileType> traversable)
        {
            edges = new Dictionary<Node, HashSet<Node>>();

            int width = levelTiles.GetLength(0);
            int height = levelTiles.GetLength(1);


            for (int ix = 0; ix < width; ix++)
            {
                for (int iy = 0; iy < height; iy++)
                {
                    for (int ox = ix; ox >= 0 && ox <= ix + 1 && ox < width; ox++)
                    {
                        for (int oy = iy; oy >= 0 && oy <= iy + 1 && oy < height; oy++)
                        {
                            // dont do for corners or center from (ix,iy)
                            if (ox - ix == oy - iy)
                            {
                                continue;
                            }

                            if (traversable.Contains(levelTiles[ix, iy].tileType) &&
                                traversable.Contains(levelTiles[ox, oy].tileType))
                            {
                                AddEdge(new Node(ix, iy), new Node(ox, oy));
                            }
                        }
                    }
                }
            }
        }

        public void AddEdge(Node node1, Node node2, bool bidir = true)
        {
            if (edges.ContainsKey(node1))
            {
                edges[node1].Add(node2);
            }
            else
            {
                edges.Add(node1, new HashSet<Node>() { node2 });
            }

            if (bidir)
            {
                AddEdge(node2, node1, false);
            }
        }
    }
}
