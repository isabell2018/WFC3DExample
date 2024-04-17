using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Tile[] upNeighbours;
    public Tile[] rightNeighbours;
    public Tile[] downNeighbours;
    public Tile[] leftNeighbours;
    public Tile[] frontNeighbours;
    public Tile[] backNeighbours;
}
