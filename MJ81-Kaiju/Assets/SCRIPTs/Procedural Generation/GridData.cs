using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridData {
    public Vector2Int coordID;
    public ZoneData parentZone;
    public List<GridData> neighbours;
    public Dictionary<Vector2Int, bool> NeighbourID;

    public float gCost, hCost;
    public float fCost { get { return gCost + hCost; } }
    public Vector2Int nodeParent;

    public GridData (Vector2Int getCoord) {
        this.coordID = getCoord;
        this.NeighbourID = new Dictionary<Vector2Int, bool> ();
    }
}