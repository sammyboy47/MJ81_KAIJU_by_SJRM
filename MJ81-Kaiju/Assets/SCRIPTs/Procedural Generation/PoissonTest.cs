using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PoissonTest : MonoBehaviour {
    public int GenSeed;
    public float radius = 1;
    public Vector2 regionSize = Vector2.one;
    public int rejectionSamples = 30;
    public float displayRadius = 1;

    public Tilemap gridEarth, gridCity, gridRoad;
    public Tile EarthTile;
    public RuleTile WaterTile, CityTile, SuburbTile, RoadTile;
    public List<Vector2> GenPoints;
    public List<GridData> gridList;

    public Dictionary<Vector2Int, GridData> gridPoints = new Dictionary<Vector2Int, GridData> (), borderCells = new Dictionary<Vector2Int, GridData> ();
    public List<ZoneData> zoneDatas;
    // Dictionary<Vector2Int, bool> zonePending = new Dictionary<Vector2Int, bool> (); //true/false if border
    void OnValidate () {
        gridEarth.ClearAllTiles ();
        gridCity.ClearAllTiles ();
        gridPoints.Clear ();
        zoneDatas.Clear ();
        GenPoints.Clear ();
        gridList.Clear ();
        GenPoints = PoissonDiscSampling.GeneratePoints (radius, regionSize, GenSeed, rejectionSamples);
        // zonePending.Clear ();
        foreach (Vector2 thisPoint in GenPoints) {
            fncGenZone (new Vector2Int ((int) thisPoint.x, (int) thisPoint.y));
            //zoneDatas.Add (new GridData (new Vector2Int ((int) thisPoint.x, (int) thisPoint.y)), );
        }
        GenerateMap ();
    }

    void GenerateMap()
    {

        GridData tempCell = new GridData(Vector2Int.zero);
        Vector2Int tempID = Vector2Int.zero;
        // Dictionary<Vector2Int, Vector2Int> pendingCheck;
        for (int py = 0; py < regionSize.y; py++)
            for (int px = 0; px < regionSize.x; px++)
            {
                tempID = new Vector2Int(px, py);
                gridPoints.Add(tempID, new GridData(tempID));
                gridPoints[tempID].parentZone = fncFindZone(tempID);

                gridList.Add(gridPoints[tempID]);

                fncGenTile(gridPoints[tempID]);

                gridRoad.SetTile(new Vector3Int(tempID.x, tempID.y, 0), null);
                // Vector2Int tempSearch = Vector2Int.zero;
                // for (int dx = -1; dx <= 1; dx++)
                //     for (int dy = -1; dy <= 1; dy++) {
                //         if (dx == 0 && dy == 0) { break; }
                //         tempSearch.x = dx + px;
                //         tempSearch.y = dy + py;
                // if (gridPoints.ContainsKey (tempSearch)) {
                //     gridPoints[tempID].NeighbourID.Add (tempSearch, false);
                // if (gridPoints[tempSearch].parentZone != null)
                //     gridPoints[tempID].NeighbourID[tempSearch] = (gridPoints[tempSearch].parentZone != gridPoints[tempID].parentZone);
                // if (gridPoints[tempSearch].parentZone != gridPoints[tempID].parentZone)
                // }
                // }
            }
        Vector2Int tempSearch = Vector2Int.zero;
        //GETTING NEIGHBOUR
        foreach (KeyValuePair<Vector2Int, GridData> thisSq in gridPoints)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) { break; }
                    tempSearch.x = dx + thisSq.Key.x;
                    tempSearch.y = dy + thisSq.Key.y;
                    if (gridPoints.ContainsKey(tempSearch))
                    {
                        thisSq.Value.NeighbourID.Add(tempSearch, false);
                        thisSq.Value.NeighbourID[tempSearch] = (thisSq.Value.parentZone == gridPoints[tempSearch].parentZone);
                        // if (!(thisSq.Value.NeighbourID[tempSearch] && gridPoints.ContainsKey(tempSearch) && borderCells.ContainsKey(tempSearch)))
                        //     borderCells.Add(tempSearch, gridPoints[tempSearch]);
                        if (!thisSq.Value.NeighbourID[tempSearch] && !borderCells.ContainsKey(tempSearch))
                        {
                            borderCells.Add(tempSearch, gridPoints[tempSearch]);
                            thisSq.Value.parentZone.fncAddNeighbour(gridPoints[tempSearch].parentZone);
                        }
                        thisSq.Value.parentZone.fncAddGrid(thisSq.Value.coordID, thisSq.Value.NeighbourID[tempSearch]);
                    }
                }
            }
        }

        foreach (ZoneData thisZone in zoneDatas)
        {
            foreach (ZoneData thisNeighbour in thisZone.zoneNeighbours)
            {
                foreach (Vector2Int thisTile in fncGetRoad(thisZone.rootPos, thisNeighbour.rootPos))
                {
                    Vector3Int getPos = new Vector3Int(thisTile.x, thisTile.y, 0);
                    if (gridRoad.GetTile(getPos) != null)
                    {
                        gridRoad.SetTile(getPos, RoadTile);
                        print($"PRINITING TILE at {thisTile}");
                    }

                }
            }
        }
    }

    void fncGenTile (GridData thisSq) {
        Vector3Int getCoord = new Vector3Int (thisSq.coordID.x, thisSq.coordID.y, 0);
        switch (thisSq.parentZone.zoneType) {
            case ZoneData.typeZone.LAND:
                gridEarth.SetTile (getCoord, EarthTile);
                break;
            case ZoneData.typeZone.WATER:
                gridEarth.SetTile (getCoord, WaterTile);
                break;
            case ZoneData.typeZone.CITY:
                gridCity.SetTile (getCoord, CityTile);
                gridEarth.SetTile (getCoord, EarthTile);
                break;
            case ZoneData.typeZone.SUBURB:
                gridCity.SetTile (getCoord, SuburbTile);
                gridEarth.SetTile (getCoord, EarthTile);
                Debug.Log($"GENERATING SUBURNS AT {thisSq.coordID}");
                break;
            default:
                break;
        }
    }
    void fncGenZone (Vector2Int getPos) {
        ZoneData.typeZone getType;
        int getRand = Random.Range (0, 4);
        switch (getRand) {
            case 0:
                getType = ZoneData.typeZone.LAND;
                break;
            case 1:
                getType = ZoneData.typeZone.WATER;
                break;
            case 2:
                getType = ZoneData.typeZone.CITY;
                break;
            case 3:
                getType = ZoneData.typeZone.SUBURB;
                break;
            default:
                getType = ZoneData.typeZone.LAND;
                break;
        }
        Debug.Log ("THIS ZONE IS " + getType);
        zoneDatas.Add (new ZoneData (getPos, getType));
    }
    ZoneData fncFindZone (Vector2Int getPos) {
        float largestDistance = Mathf.Infinity, measureDistance;
        ZoneData getZone = zoneDatas[0];
        foreach (ZoneData thisZone in zoneDatas) {
            measureDistance = (thisZone.rootPos - getPos).sqrMagnitude;
            if (largestDistance > measureDistance) {
                largestDistance = measureDistance;
                getZone = thisZone;
            }
        }
        return getZone;
    }

    List<Vector2Int> fncGetRoad(Vector2Int startPos, Vector2Int endPos)
    {
        List<Vector2Int> getPath = new List<Vector2Int>(),
        //
         openSet = new List<Vector2Int>(), closedSet = new List<Vector2Int>();
        openSet.Add(startPos);
        float getDist = 0, newCostNeigh;
        Vector2Int curPos, curNeigh;
        Dictionary<Vector2Int, float> costList = new Dictionary<Vector2Int, float>();
        Dictionary<Vector2Int, Vector2Int> getPrevNode = new Dictionary<Vector2Int, Vector2Int>();
        while (openSet.Count > 0)
        {
            curPos = openSet[0];
            foreach (Vector2Int thisPoint in openSet)
            {
                if (gridPoints[thisPoint].fCost <= gridPoints[curPos].fCost)
                    if (gridPoints[thisPoint].hCost < gridPoints[curPos].hCost)
                        curPos = thisPoint;
            }
            openSet.Remove(curPos);
            closedSet.Add(curPos);
            //
            if (curPos == endPos) break;
            //
            foreach (KeyValuePair<Vector2Int, bool> thisPoint in gridPoints[curPos].NeighbourID)
            {
                curNeigh = thisPoint.Key;
                if ((gridPoints[thisPoint.Key].parentZone.zoneType == ZoneData.typeZone.WATER)
                || closedSet.Contains(thisPoint.Key)
                )
                { continue; }
                newCostNeigh = gridPoints[curPos].gCost + (curPos - thisPoint.Key).sqrMagnitude; //(thisPoint.Key - startPos).sqrMagnitude + (endPos - thisPoint.Key).sqrMagnitude;
                newCostNeigh = newCostNeigh * (gridRoad.GetTile(new Vector3Int(curNeigh.x, curNeigh.y, 0)) != null ? .5f : 1) * (borderCells.ContainsKey(curNeigh) ? .5f : 1);
                if (newCostNeigh < gridPoints[curNeigh].gCost || !openSet.Contains(thisPoint.Key))
                {
                    gridPoints[curNeigh].gCost = newCostNeigh;
                    gridPoints[curNeigh].hCost = (endPos - curNeigh).sqrMagnitude;
                    gridPoints[curNeigh].nodeParent = curPos;

                    if (!openSet.Contains(thisPoint.Key))
                        openSet.Add(thisPoint.Key);
                }
            }
        }

        Vector2Int checkNode = endPos;
        while (checkNode != startPos)
        {
            getPath.Add(checkNode);
            checkNode = gridPoints[checkNode].nodeParent;
        }

        //
        return getPath;
    }
    void OnDrawGizmos () {
        Gizmos.DrawWireCube (regionSize / 2, regionSize);
        if (GenPoints != null) {
            Gizmos.color = Color.green;
            foreach (Vector2 point in GenPoints) {
                Gizmos.DrawSphere (point, displayRadius);
            }
        }
    }
}