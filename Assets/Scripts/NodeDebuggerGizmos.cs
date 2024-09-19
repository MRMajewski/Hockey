using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeDebuggerGizmos : MonoBehaviour
{
    public BallMovement ballMovement; // Referencja do skryptu BallMovement
    public float gizmoRadius = 0.5f; // Promie� sfery gizmo

    private void OnDrawGizmos()
    {
        if (ballMovement != null)
        {
            Node currentNode = ballMovement.GetConfirmedNode();
            if (currentNode != null)
            {
                // Ustaw kolor gizmo na zielony
                Gizmos.color = Color.green;

                // Rysuj sfer� gizmo na pozycji aktualnego w�z�a
                Gizmos.DrawSphere(currentNode.Position, gizmoRadius);
            }
        }
    }
}
