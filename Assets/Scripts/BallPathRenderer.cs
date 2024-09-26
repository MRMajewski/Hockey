using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BallPathRenderer : MonoBehaviour
{
    [SerializeField]
    private GameObject lineSegmentPrefab;

    private HashSet<(Node, Node)> drawnPaths = new HashSet<(Node, Node)>();

    [SerializeField]
    private Transform pathsPrefabParent;

    public void AddPosition(ref Node fromNode, ref Node toNode, Color color)
    {
        CreateLineSegment(fromNode, toNode, color);
        MarkConnectionAsUsed(fromNode, toNode);
    }
    public bool IsMoveLegal(Node startNode, Node endNode)
    {
        if (!startNode.IsNeighbor(endNode))
        {
            return false;
        }
        if (startNode.Position.Equals(endNode.Position))
        {
            return false;
        }

        if (drawnPaths.Contains((startNode, endNode)) || drawnPaths.Contains((endNode, startNode)))
        {
            return false;
        }

        if (DoesDiagonalMoveIntersect(startNode, endNode))
        {
            return false;
        }

        return true;
    }

    public bool WasNodeAlreadyUsed(Node targetNode)
    {
        for (int i = 0; i < drawnPaths.Count-2; i++)
        {
            var path = drawnPaths.ElementAt(i);

            if (path.Item1 == targetNode || path.Item2 == targetNode)
            {
                return true; 
            }
        }
        return false;
    }

    private bool DoesDiagonalMoveIntersect(Node startNode, Node endNode)
    {
        if (startNode.Position.x == endNode.Position.x || startNode.Position.y == endNode.Position.y)
        {
            return false;
        }

        Vector2 startPos = startNode.Position;
        Vector2 endPos = endNode.Position;

        foreach (var path in drawnPaths)
        {
            Vector2 pathStart = path.Item1.Position;
            Vector2 pathEnd = path.Item2.Position;

            if ((startPos == pathStart && endPos == pathEnd) || (startPos == pathEnd && endPos == pathStart))
            {
                continue;
            }

            if (LineIntersects(startPos, endPos, pathStart, pathEnd))
            {
                return true; 
            }
        }

        return false;
    }
    private bool LineIntersects(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
    {
        float a1 = end1.y - start1.y;
        float b1 = start1.x - end1.x;
        float c1 = a1 * start1.x + b1 * start1.y;

        float a2 = end2.y - start2.y;
        float b2 = start2.x - end2.x;
        float c2 = a2 * start2.x + b2 * start2.y;

        float determinant = a1 * b2 - a2 * b1;

        if (determinant == 0)
        {
            return false;
        }

        float x = (b2 * c1 - b1 * c2) / determinant;
        float y = (a1 * c2 - a2 * c1) / determinant;
        Vector2 intersectionPoint = new Vector2(x, y);

        bool isOnSegment1 = IsPointOnSegment(intersectionPoint, start1, end1, true);
        bool isOnSegment2 = IsPointOnSegment(intersectionPoint, start2, end2, true);

        return isOnSegment1 && isOnSegment2;
    }

    private bool IsPointOnSegment(Vector2 point, Vector2 segmentStart, Vector2 segmentEnd, bool excludeEndpoints = false)
    {
        float minX = Mathf.Min(segmentStart.x, segmentEnd.x);
        float maxX = Mathf.Max(segmentStart.x, segmentEnd.x);
        float minY = Mathf.Min(segmentStart.y, segmentEnd.y);
        float maxY = Mathf.Max(segmentStart.y, segmentEnd.y);

        bool isOnSegment = (minX < point.x && point.x < maxX) &&
                           (minY < point.y && point.y < maxY);

        if (excludeEndpoints)
        {
            return isOnSegment && (point != segmentStart && point != segmentEnd);
        }
        return isOnSegment;
    }

    private void CreateLineSegment(Node fromNode, Node toNode, Color color)
    {
        Vector2 from = fromNode.Position;
        Vector2 to = toNode.Position;

        GameObject segment = Instantiate(lineSegmentPrefab, pathsPrefabParent);
        Vector2 midPoint = (from + to) / 2;
        segment.transform.localPosition = new Vector3(midPoint.x, midPoint.y, 0);

        float distance = Vector3.Distance(from, to);
        segment.transform.localScale = new Vector3(distance, .2f, 1);

        float angle = Mathf.Atan2(to.y - from.y, to.x - from.x) * Mathf.Rad2Deg;
        segment.transform.rotation = Quaternion.Euler(0, 0, angle);

        var spriteRenderer = segment.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }

        drawnPaths.Add((fromNode, toNode));
        drawnPaths.Add((toNode, fromNode));
    }

    public void ClearPaths()
    {
        foreach (Transform child in pathsPrefabParent)
        {
            Destroy(child.gameObject);
        }
        drawnPaths.Clear();
        drawnPaths.TrimExcess();
        Debug.Log("All paths cleared");
    }

    public void MarkConnectionAsUsed(Node nodeA, Node nodeB)
    {
        drawnPaths.Add((nodeA, nodeB));
        drawnPaths.Add((nodeB, nodeA)); 
    }
}
