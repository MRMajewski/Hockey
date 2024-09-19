using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public BallMovement ballMovement;
    public BallPathRenderer pathRenderer;
    public AIController aiController;

    private Node lastNode;  // Wêze³ zamiast Vector3
    private Color playerColor = Color.blue;
    private Color aiColor = Color.red;

    public bool isPlayerTurn = true;

    //void Start()
    //{
    //    // Ustaw pocz¹tkowy wêze³ na podstawie pozycji pi³ki
    //    lastNode = ballMovement.GetCurrentNode();  // U¿ywamy metody do uzyskania aktualnego wêz³a
    //    isPlayerTurn = true;
    //}

    public void TurnManagerInit()
    {
        lastNode = ballMovement.GetCurrentNode();  // U¿ywamy metody do uzyskania aktualnego wêz³a
        isPlayerTurn = true;
    }

    void Update()
    {
        //  if (isPlayerTurn)
        //    {
        HandlePlayerTurn();
        //}
        //else
        //{
        //    PerformAITurn();
        //}
    }

    void HandlePlayerTurn()
    {
        if (Input.GetButtonDown("Submit"))
        {
            Debug.Log("Dzia³a Enter");

            Node currentNode = ballMovement.GetCurrentNode();  // Pobierz aktualny wêze³

            if (pathRenderer.IsMoveLegal(lastNode, currentNode))
            {
                pathRenderer.AddPosition(lastNode, currentNode, playerColor);
                lastNode = currentNode;  // Zaktualizuj globaln¹ zmienn¹ lastNode
                isPlayerTurn = false;
                Debug.Log("Koniec tury gracza!");

                PerformAITurn();
            }
            else
            {
                // Jeœli ruch jest nielegalny, zresetuj pozycjê pi³ki do poprzedniego wêz³a
                ballMovement.SetCurrentNode(lastNode);

                ballMovement.MoveBall();  // Zaktualizuj pozycjê pi³ki w œwiecie
            }
        }
    }

    void PerformAITurn()
    {
        if (aiController != null)
        {
            aiController.PerformAITurn();

            Debug.Log("Koniec tury AI!");
        }

        isPlayerTurn = true;
    }
}
