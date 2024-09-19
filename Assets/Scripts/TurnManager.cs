using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public BallMovement ballMovement;
    public BallPathRenderer pathRenderer;
    public AIController aiController;

    private Node lastNode;  // W�ze� zamiast Vector3
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
        lastNode = ballMovement.GetCurrentNode();  // U�ywamy metody do uzyskania aktualnego w�z�a
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
            Debug.Log("Dzia�a Enter");

            Node currentNode = ballMovement.GetCurrentNode();  // Pobierz aktualny w�ze�


            Debug.Log("lastNode pos: "+ lastNode.Position);

            Debug.Log("currentNode pos: " + currentNode.Position);

            Node targetNode = ballMovement.GetTargetNode();
        //    Node previousNode= ballMovement.GetNodeAtPosition((Vector2)ballMovement.ball.transform.position);

       //     Debug.Log("previousNode pos: " + previousNode.Position);

            if (pathRenderer.IsMoveLegal(currentNode, targetNode))
            //    if (pathRenderer.IsMoveLegal(lastNode, currentNode))
                {
             
              //  pathRenderer.AddPosition(lastNode, currentNode, playerColor);
                pathRenderer.AddPosition(currentNode, targetNode, playerColor);
                lastNode = currentNode;  // Zaktualizuj globaln� zmienn� lastNode
                isPlayerTurn = false;
                Debug.Log("Koniec tury gracza!");
                ballMovement.SetCurrentNode(currentNode);
                PerformAITurn();
            }
            else
            {
                // Je�li ruch jest nielegalny, zresetuj pozycj� pi�ki do poprzedniego w�z�a
             //   ballMovement.SetCurrentNode(lastNode);
                ballMovement.SetCurrentNode(currentNode);
                ballMovement.MoveBall();  // Zaktualizuj pozycj� pi�ki w �wiecie
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
