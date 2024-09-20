using UnityEngine;

public class ArenaGenerator : MonoBehaviour
{
    public GameObject gridCellPrefab;
    public Transform parent;

    [Range(2, 10)]
    public int width = 8;
    [Range(2, 10)]
    public int height = 10;

    public float cellSize = 1.0f;
    public Vector2 offset = new Vector2(0.5f, 0.5f);
    public GameObject goalPrefab;
    public float goalWidth = 2.0f;

    [SerializeField]
    private GridManager gridManager;

    private void OnValidate()
    {
        if (width % 2 != 0)
        {
            width = Mathf.Max(2, width - 1);
        }
        if (height % 2 != 0)
        {
            height = Mathf.Max(2, height - 1);
        }
    }

    public void GenerateArena()
    {
        if (parent == null)
        {
            Debug.LogError("Parent transform is not assigned!");
            return;
        }

        Debug.Log("Clearing arena...");
        ClearArena();

        Debug.Log("Generating grid...");
        Vector3 basePosition = new Vector3(-width * cellSize / 2 + offset.x, -height * cellSize / 2 + offset.y, 0);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = basePosition + new Vector3(x * cellSize, y * cellSize, 0);
                Instantiate(gridCellPrefab, position, Quaternion.identity, parent);
            }
        }

        Debug.Log("Adding goals...");
        AddGoals();

        Debug.Log("Generating nodes...");
        gridManager.GenerateNodes(width, height);

        Debug.Log("Arena generated.");
    }

    void AddGoals()
    {
        if (goalPrefab == null)
        {
            Debug.LogError("Goal prefab is not assigned!");
            return;
        }

        float halfWidth = width * cellSize / 2;
        float halfHeight = height * cellSize / 2;

        Vector3 topGoalPosition = new Vector3(0, halfHeight + cellSize / 2, 0);
        Vector3 bottomGoalPosition = new Vector3(0, -halfHeight - cellSize / 2, 0);

        for (int i = 0; i < goalWidth; i++)
        {
            Instantiate(goalPrefab, topGoalPosition + new Vector3((i - (goalWidth - 1) / 2f) * cellSize, 0, 0), Quaternion.identity, parent);
            Instantiate(goalPrefab, bottomGoalPosition + new Vector3((i - (goalWidth - 1) / 2f) * cellSize, 0, 0), Quaternion.identity, parent);
        }
    }

    public void ClearArena()
    {
        if (parent != null)
        {
            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }
        }
    }
}