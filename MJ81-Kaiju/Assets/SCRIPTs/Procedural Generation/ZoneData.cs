using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ZoneData
{
    public Dictionary<Vector2Int, bool> gridList;
    public List<ZoneData> zoneNeighbours;
    public typeZone zoneType = typeZone.NONE;
    public Vector2Int rootPos;
    public enum typeZone { NONE, CITY, LAND, SUBURB, WATER }

    public ZoneData(Vector2Int getPos, typeZone getType = typeZone.NONE)
    {
        this.rootPos = getPos;
        zoneType = getType;
        gridList = new Dictionary<Vector2Int, bool>();
        zoneNeighbours = new List<ZoneData>();
    }
    public void fncAddGrid(Vector2Int getPos, bool isBorder = false)
    {
        if (!gridList.ContainsKey(getPos))
            this.gridList.Add(getPos, isBorder);
        else
            this.gridList[getPos] = isBorder;
    }
    public void fncRemGrid(Vector2Int getPos)
    {
        gridList.Remove(getPos);
    }
    public void fncAddNeighbour(ZoneData thisNeighbour)
    {
        if (!zoneNeighbours.Contains(thisNeighbour))
            this.zoneNeighbours.Add(thisNeighbour);
    }
    public void fncRemNeighbour(ZoneData thisNeighbour)
    {
        this.zoneNeighbours.Remove(thisNeighbour);
    }
}