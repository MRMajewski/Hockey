using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BallPathRenderer : MonoBehaviour
{
    [SerializeField]
    private GameObject lineSegmentPrefab;

    private HashSet<(Node, Node)> drawnPaths = new HashSet<(Node, Node)>();

    public void AddPosition(Node fromNode, Node toNode, Color color)
    {
     //   if (IsMoveLegal(fromNode, toNode))
        {
            CreateLineSegment(fromNode, toNode, color);
            MarkConnectionAsUsed(fromNode, toNode);
        }
     //   else
      //  {
      //      Debug.Log("Ruch nielegalny: Przeciêcie œcie¿ki.");
      //  }
    }
    public bool IsMoveLegal(Node startNode, Node endNode)
    {
        // 1. SprawdŸ, czy endNode jest s¹siadem startNode (ruch w osi x lub y)
        if (!startNode.IsNeighbor(endNode))
        {
            Debug.Log("Ruch niedozwolony: endNode nie jest s¹siadem startNode.");
            return false;
        }

        // 2. SprawdŸ, czy istnieje ju¿ taka sama œcie¿ka w drawnPaths
        if (drawnPaths.Contains((startNode, endNode)) || drawnPaths.Contains((endNode, startNode)))
        {
            Debug.Log("Ruch niedozwolony: Istnieje ju¿ po³¹czenie miêdzy startNode a endNode.");
            return false;
        }

        // 3. SprawdŸ, czy skosowy ruch przecina istniej¹ce œcie¿ki w drawnPaths
        if (DoesDiagonalMoveIntersect(startNode, endNode))
        {
            Debug.Log("Ruch niedozwolony: Skosowy ruch przecina istniej¹c¹ œcie¿kê.");
            return false;
        }

        Debug.Log("Ruch legalny.");
        return true;
    }

    private bool DoesDiagonalMoveIntersect(Node startNode, Node endNode)
    {
        // Jeœli ruch nie jest skosowy, nie ma potrzeby sprawdzaæ przeciêcia
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

            // Jeœli œcie¿ka jest bezpoœrednim po³¹czeniem, pomijamy
            if ((startPos == pathStart && endPos == pathEnd) || (startPos == pathEnd && endPos == pathStart))
            {
                continue;
            }

            // SprawdŸ przeciêcie œcie¿ki z istniej¹cymi œcie¿kami
            if (LineIntersects(startPos, endPos, pathStart, pathEnd))
            {
                return true; // Skosowy ruch przecina istniej¹c¹ œcie¿kê
            }
        }

        return false;
    }
    private bool LineIntersects(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
    {
        // SprawdŸ, czy odcinki s¹ równoleg³e
        float a1 = end1.y - start1.y;
        float b1 = start1.x - end1.x;
        float c1 = a1 * start1.x + b1 * start1.y;

        float a2 = end2.y - start2.y;
        float b2 = start2.x - end2.x;
        float c2 = a2 * start2.x + b2 * start2.y;

        float determinant = a1 * b2 - a2 * b1;

        if (determinant == 0)
        {
            // Linie s¹ równoleg³e i nie przecinaj¹ siê
            return false;
        }

        // Oblicz punkt przeciêcia
        float x = (b2 * c1 - b1 * c2) / determinant;
        float y = (a1 * c2 - a2 * c1) / determinant;
        Vector2 intersectionPoint = new Vector2(x, y);

        // SprawdŸ, czy punkt przeciêcia le¿y w obrêbie obu odcinków, z wyj¹tkiem koñców
        bool isOnSegment1 = IsPointOnSegment(intersectionPoint, start1, end1, true);
        bool isOnSegment2 = IsPointOnSegment(intersectionPoint, start2, end2, true);

        return isOnSegment1 && isOnSegment2;
    }

    private bool IsPointOnSegment(Vector2 point, Vector2 segmentStart, Vector2 segmentEnd, bool excludeEndpoints = false)
    {
        // SprawdŸ, czy punkt jest wzd³u¿ odcinka
        float minX = Mathf.Min(segmentStart.x, segmentEnd.x);
        float maxX = Mathf.Max(segmentStart.x, segmentEnd.x);
        float minY = Mathf.Min(segmentStart.y, segmentEnd.y);
        float maxY = Mathf.Max(segmentStart.y, segmentEnd.y);

        bool isOnSegment = (minX < point.x && point.x < maxX) &&
                           (minY < point.y && point.y < maxY);

        if (excludeEndpoints)
        {
            // Jeœli koñce maj¹ byæ wykluczone, musimy upewniæ siê, ¿e punkt nie jest koñcem segmentu
            return isOnSegment && (point != segmentStart && point != segmentEnd);
        }

        return isOnSegment;
    }

    //private bool IsPointOnSegment(Vector2 point, Vector2 segmentStart, Vector2 segmentEnd)
    //{
    //    // SprawdŸ, czy punkt jest wzd³u¿ odcinka
    //    return (Mathf.Min(segmentStart.x, segmentEnd.x) <= point.x && point.x <= Mathf.Max(segmentStart.x, segmentEnd.x)) &&
    //           (Mathf.Min(segmentStart.y, segmentEnd.y) <= point.y && point.y <= Mathf.Max(segmentStart.y, segmentEnd.y));
    //}

    private bool DoesPathContainUsedNode(Node startNode, Node endNode)
    {
        foreach (var path in drawnPaths)
        {
            if (path.Item1 == startNode || path.Item2 == startNode ||
                path.Item1 == endNode || path.Item2 == endNode)
            {
                return true;
            }
        }
        return false;
    }

    private void CreateLineSegment(Node fromNode, Node toNode, Color color)
    {
        Vector3 from = fromNode.Position;
        Vector3 to = toNode.Position;

        GameObject segment = Instantiate(lineSegmentPrefab, transform);
        Vector3 midPoint = (from + to) / 2;
        segment.transform.position = midPoint;

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
        drawnPaths.Add((toNode, fromNode)); // Dodajemy równie¿ w odwrotnej kolejnoœci
    }

    private bool DoesLineIntersect(Node startNode, Node endNode)
    {
        // ZnajdŸ wspólne s¹siedztwo, ¿eby sprawdziæ przeciêcia
        var commonNeighbors = startNode.Neighbors.Intersect(endNode.Neighbors).ToList();

        foreach (var neighbor in commonNeighbors)
        {
            if (drawnPaths.Contains((neighbor, startNode)) || drawnPaths.Contains((startNode, neighbor)) ||
                drawnPaths.Contains((neighbor, endNode)) || drawnPaths.Contains((endNode, neighbor)))
            {
                return true; // Znaleziono przeciêcie
            }
        }
        return false;
    }

    public void ClearPaths()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        drawnPaths.Clear();
        drawnPaths.TrimExcess();
        Debug.Log("All paths cleared");
    }

    public void MarkConnectionAsUsed(Node nodeA, Node nodeB)
    {
        drawnPaths.Add((nodeA, nodeB));
        drawnPaths.Add((nodeB, nodeA)); // Dodajemy równie¿ w odwrotnej kolejnoœci
    }
}
