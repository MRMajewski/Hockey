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
      //      Debug.Log("Ruch nielegalny: Przeci�cie �cie�ki.");
      //  }
    }
    public bool IsMoveLegal(Node startNode, Node endNode)
    {
        // 1. Sprawd�, czy endNode jest s�siadem startNode (ruch w osi x lub y)
        if (!startNode.IsNeighbor(endNode))
        {
            Debug.Log("Ruch niedozwolony: endNode nie jest s�siadem startNode.");
            return false;
        }

        // 2. Sprawd�, czy istnieje ju� taka sama �cie�ka w drawnPaths
        if (drawnPaths.Contains((startNode, endNode)) || drawnPaths.Contains((endNode, startNode)))
        {
            Debug.Log("Ruch niedozwolony: Istnieje ju� po��czenie mi�dzy startNode a endNode.");
            return false;
        }

        // 3. Sprawd�, czy skosowy ruch przecina istniej�ce �cie�ki w drawnPaths
        if (DoesDiagonalMoveIntersect(startNode, endNode))
        {
            Debug.Log("Ruch niedozwolony: Skosowy ruch przecina istniej�c� �cie�k�.");
            return false;
        }

        Debug.Log("Ruch legalny.");
        return true;
    }

    private bool DoesDiagonalMoveIntersect(Node startNode, Node endNode)
    {
        // Je�li ruch nie jest skosowy, nie ma potrzeby sprawdza� przeci�cia
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

            // Je�li �cie�ka jest bezpo�rednim po��czeniem, pomijamy
            if ((startPos == pathStart && endPos == pathEnd) || (startPos == pathEnd && endPos == pathStart))
            {
                continue;
            }

            // Sprawd� przeci�cie �cie�ki z istniej�cymi �cie�kami
            if (LineIntersects(startPos, endPos, pathStart, pathEnd))
            {
                return true; // Skosowy ruch przecina istniej�c� �cie�k�
            }
        }

        return false;
    }
    private bool LineIntersects(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
    {
        // Sprawd�, czy odcinki s� r�wnoleg�e
        float a1 = end1.y - start1.y;
        float b1 = start1.x - end1.x;
        float c1 = a1 * start1.x + b1 * start1.y;

        float a2 = end2.y - start2.y;
        float b2 = start2.x - end2.x;
        float c2 = a2 * start2.x + b2 * start2.y;

        float determinant = a1 * b2 - a2 * b1;

        if (determinant == 0)
        {
            // Linie s� r�wnoleg�e i nie przecinaj� si�
            return false;
        }

        // Oblicz punkt przeci�cia
        float x = (b2 * c1 - b1 * c2) / determinant;
        float y = (a1 * c2 - a2 * c1) / determinant;
        Vector2 intersectionPoint = new Vector2(x, y);

        // Sprawd�, czy punkt przeci�cia le�y w obr�bie obu odcink�w, z wyj�tkiem ko�c�w
        bool isOnSegment1 = IsPointOnSegment(intersectionPoint, start1, end1, true);
        bool isOnSegment2 = IsPointOnSegment(intersectionPoint, start2, end2, true);

        return isOnSegment1 && isOnSegment2;
    }

    private bool IsPointOnSegment(Vector2 point, Vector2 segmentStart, Vector2 segmentEnd, bool excludeEndpoints = false)
    {
        // Sprawd�, czy punkt jest wzd�u� odcinka
        float minX = Mathf.Min(segmentStart.x, segmentEnd.x);
        float maxX = Mathf.Max(segmentStart.x, segmentEnd.x);
        float minY = Mathf.Min(segmentStart.y, segmentEnd.y);
        float maxY = Mathf.Max(segmentStart.y, segmentEnd.y);

        bool isOnSegment = (minX < point.x && point.x < maxX) &&
                           (minY < point.y && point.y < maxY);

        if (excludeEndpoints)
        {
            // Je�li ko�ce maj� by� wykluczone, musimy upewni� si�, �e punkt nie jest ko�cem segmentu
            return isOnSegment && (point != segmentStart && point != segmentEnd);
        }

        return isOnSegment;
    }

    //private bool IsPointOnSegment(Vector2 point, Vector2 segmentStart, Vector2 segmentEnd)
    //{
    //    // Sprawd�, czy punkt jest wzd�u� odcinka
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
        drawnPaths.Add((toNode, fromNode)); // Dodajemy r�wnie� w odwrotnej kolejno�ci
    }

    private bool DoesLineIntersect(Node startNode, Node endNode)
    {
        // Znajd� wsp�lne s�siedztwo, �eby sprawdzi� przeci�cia
        var commonNeighbors = startNode.Neighbors.Intersect(endNode.Neighbors).ToList();

        foreach (var neighbor in commonNeighbors)
        {
            if (drawnPaths.Contains((neighbor, startNode)) || drawnPaths.Contains((startNode, neighbor)) ||
                drawnPaths.Contains((neighbor, endNode)) || drawnPaths.Contains((endNode, neighbor)))
            {
                return true; // Znaleziono przeci�cie
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
        drawnPaths.Add((nodeB, nodeA)); // Dodajemy r�wnie� w odwrotnej kolejno�ci
    }
}
