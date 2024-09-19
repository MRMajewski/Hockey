using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public BallMovement ballMovement;
    public BallPathRenderer pathRenderer;
    public AIController aiController;

    private Node lastConfirmedNode;
    private Color playerColor = Color.blue;
    private Color aiColor = Color.red;

    public bool isPlayerTurn = true;

    //void Start()
    //{
    //    // Ustaw pocz�tkowy w�ze� na podstawie pozycji pi�ki
    //    lastNode = ballMovement.GetCurrentNode();  // U�ywamy metody do uzyskania aktualnego w�z�a
    //    isPlayerTurn = true;
    //}

    public void TurnManagerInit()
    {
        lastConfirmedNode = ballMovement.GetConfirmedNode(); // Ustawiamy pierwszy zatwierdzony w�ze�
        isPlayerTurn = true;
    }
    void Update()
    {
        if (isPlayerTurn)
        HandlePlayerTurn();
    }

    void HandlePlayerTurn()
    {
        if (Input.GetButtonDown("Submit"))
        {
            Debug.Log("Dzia�a Enter");
            Debug.Log("currentTemporaryNode Position w HandlePlayer" + ballMovement.GetTargetNode().Position);
            Debug.Log("currentTemporaryNode Position w HandlePlayer" + ballMovement.currentTemporaryNode.Position);

            Node currentNode = ballMovement.GetConfirmedNode(); // Tymczasowy w�ze� pi�ki
        //    Node targetNode = ballMovement.GetTargetNode(); // Ostateczny w�ze�, kt�ry chcemy zatwierdzi�
            Node targetNode = ballMovement.currentTemporaryNode;

            if (pathRenderer.IsMoveLegal(currentNode, targetNode))
                {

                pathRenderer.AddPosition(currentNode, targetNode, playerColor);
                lastConfirmedNode = targetNode;
                isPlayerTurn = false;
                Debug.Log("Koniec tury gracza!");
                ballMovement.SetConfirmedNode(currentNode);
                PerformAITurn();
            }
            else
            {

                ballMovement.ResetToLastConfirmedNode();
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
