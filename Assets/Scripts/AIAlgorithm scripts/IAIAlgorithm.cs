using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAIAlgorithm
{
    // Metoda do wyboru najlepszego ruchu na podstawie obecnego i docelowego wêz³a
    Node GetBestMove(Node currentNode, Node goalNode);
}
