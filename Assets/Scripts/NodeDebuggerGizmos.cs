using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeDebuggerGizmos : MonoBehaviour
{
    public BallMovement ballMovement; 
    public float gizmoRadius = 0.5f;

    private void OnDrawGizmos()
    {
        if (ballMovement != null)
        {
            Node currentNode = ballMovement.GetConfirmedNode();
            if (currentNode != null)
            {
                Gizmos.color = Color.green;

                Gizmos.DrawSphere(currentNode.Position, gizmoRadius);
            }

            Node tempNode = ballMovement.GetTargetNode();
            if (tempNode != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(tempNode.Position, gizmoRadius/2);
            }
        }
    }
}
