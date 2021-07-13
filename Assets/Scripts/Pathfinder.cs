using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
    public LevelManagerMB levelMngr;

    public GameObject source;
    public GameObject target;

    public float radius;

    public Dictionary<LevelAnalysis.Node, LevelAnalysis.Node> parentMap;
    public Queue<Vector2Int> nextMoves;
    public Vector2Int nextMove;

    public Pathfinder(GameObject source, GameObject target, float radius)
    {
        this.source = source;
        this.target = target;
        this.radius = radius;
        levelMngr = GameManagerMB.instance.levelMngr;
        parentMap = null;
        nextMoves = new Queue<Vector2Int>();
        nextMove = new Vector2Int(-1, -1); // force update when chekced
    }

    public bool CanSeeTarget()
    {
        var hits = new List<RaycastHit2D>();
        Vector2 toTarget = target.transform.position - source.transform.position;
        Vector2 perpvec = Quaternion.Euler(0, 0, 90) * toTarget;
        Vector2 pos1 = (Vector2)source.transform.position + perpvec.normalized * radius;
        Vector2 pos2 = (Vector2)source.transform.position - perpvec.normalized * radius;

        Physics2D.Raycast(pos1, toTarget, new ContactFilter2D().NoFilter(), hits, toTarget.magnitude);

        foreach (var hit in hits)
        {
            if (hit.transform.tag == "Wall")
            {
                return false;
            }
        }
        Physics2D.Raycast(pos2, toTarget, new ContactFilter2D().NoFilter(), hits, toTarget.magnitude);

        foreach (var hit in hits)
        {
            if (hit.transform.tag == "Wall")
            {
                return false;
            }
        }

        return true;
    }

    public Vector2 GetMoveDir()
    {
        if (CanSeeTarget())
        {
            return (target.transform.position - source.transform.position).normalized;
        }

        (int x, int y) = levelMngr.level.GridLocation(source.transform.position) ?? (-1, -1);
        if (x < 0 || y < 0)
        {
            Debug.Log("Source position not found in grid");
            return Vector2.zero;
        }

        Vector2Int currentPos = new Vector2Int(x, y);

        if (nextMoves.Count == 0 || (currentPos - nextMove).magnitude > 1.1f)
        {
            (int tx, int ty) = levelMngr.level.GridLocation(target.transform.position) ?? (-1, -1);
            if (tx < 0 || ty < 0)
            {
                Debug.Log("Target position not found in grid");
                return Vector2.zero;
            }
            nextMoves.Clear();
            foreach (Vector2Int v in LevelAnalysis.PathToPlayer(levelMngr.la_graph, new LevelAnalysis.Node(x, y), new LevelAnalysis.Node(tx, ty)) ?? new List<Vector2Int>())
            {
                nextMoves.Enqueue(v);
            }
            if (nextMoves.Count != 0) nextMove = nextMoves.Dequeue();
            Debug.Log("newpath");
        }

        if (currentPos == nextMove)
        {
            if (nextMoves.Count != 0) nextMove = nextMoves.Dequeue();
            Debug.Log("moved");
            Debug.Log(string.Format("{0}, {1}", currentPos, nextMove));
        }
        if (nextMoves.Count == 0) return Vector2.zero; // kinda causes a 1 frame delay after reaching end of nextMoves
        else return (levelMngr.level.WorldLocation(nextMove) - (Vector2)source.transform.position).normalized;
    }
}
