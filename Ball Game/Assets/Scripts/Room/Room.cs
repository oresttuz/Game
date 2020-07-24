using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room
{
    public enum TileType
    {
        empty,
        floor,
        wall,
        door
    }

    public int doorways;
    
    public TileType[,] tileDataGrid;

    public Room()
    {
        doorways = 0;
        tileDataGrid = new TileType[8, 8];
    }

    public Room(int d)
    {
        doorways = d;
        tileDataGrid = new TileType[8, 8];
    }

    public Room(int x, int y)
    {
        doorways = 0;
        tileDataGrid = new TileType[x, y];
    }

    public Room(int d, int x, int y)
    {
        doorways = d;
        tileDataGrid = new TileType[x, y];
    }

    public List<Vector3Int> CreateRoom(Vector3Int roomSize, int boundary)
    {
        List<Walker> floorWalkers = new List<Walker>();
        List<Vector3Int> initTilePos = new List<Vector3Int>();

        Vector3Int origin = new Vector3Int(Random.Range(boundary, roomSize.x - boundary), Random.Range(boundary, roomSize.y - boundary), 0);

        floorWalkers.Add(new Walker(origin, new Vector3Int(boundary, boundary, 0), new Vector3Int(roomSize.x - boundary, roomSize.y - boundary, 0)));
        initTilePos.Add(origin);
        int tilesToAdd = Mathf.FloorToInt(Random.Range(0.5f, 0.75f) * (roomSize.x - (2 * boundary)) * (roomSize.y - (2 * boundary)));
        int currNumTiles = 1;

        while (currNumTiles < tilesToAdd)
        {
            Vector3Int temp;
            foreach (Walker w in floorWalkers)
            {
                if (currNumTiles >= tilesToAdd)
                {
                    Debug.Log("Full Room");
                    break;
                }
                temp = w.Step();
                if (temp != null)
                {
                    if (initTilePos.Contains(temp))
                    {
                        //Debug.Log("Passing by");
                    }
                    else
                    {
                        initTilePos.Add(temp);
                        tileDataGrid[temp.x, temp.y] = TileType.floor;
                        currNumTiles++;
                    }
                }
            }
        }
        return initTilePos;
    }  
}



