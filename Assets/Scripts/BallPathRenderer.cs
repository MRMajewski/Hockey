using UnityEngine;
using System.Collections.Generic;

public class BallPathRenderer : MonoBehaviour
{
    [SerializeField]
    private GameObject lineSegmentPrefab;

    private List<(Vector3 start, Vector3 end)> lineSegments = new List<(Vector3 start, Vector3 end)>();

    public void AddPosition(Vector3 from, Vector3 to, Color color)
    {
        CreateLineSegment(from, to, color);
    }

    void CreateLineSegment(Vector3 from, Vector3 to, Color color)
    {
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

        lineSegments.Add((from, to));
    }

    public void ClearPaths()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        Debug.Log("All paths cleared");
    }
}