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

    public int boundary;
    public bool[] doorways = {false, false, false, false };
    public TileType[,] RoomGrid;

    private float randomFill;

    public Room()
    {
        boundary = 0;
        RoomGrid = new TileType[8, 8];
        randomFill = 0.5f;
    }

    public Room(int b)
    {
        boundary = b;
        RoomGrid = new TileType[8, 8];
        randomFill = 0.5f;
    }

    public Room(Vector3Int dim)
    {
        boundary = 0;
        RoomGrid = new TileType[dim.x, dim.y];
        randomFill = 0.5f;
    }

    public Room(int b, Vector3Int dim)
    {
        boundary = b;
        RoomGrid = new TileType[dim.x, dim.y];
        randomFill = 0.5f;
    }

    public void AddDoorway(int door)
    {
        switch (door)
        {
            case 0:
                doorways[door] = true;
                break;
            case 1:
                doorways[door] = true;
                break;
            case 2:
                doorways[door] = true;
                break;
            case 3:
                doorways[door] = true;
                break;
            default:
                break;
        }
    }

    public List<Vector3Int> CreateFloors(Vector3Int roomSize)
    {
        List<Walker> floorWalkers = new List<Walker>();
        List<Vector3Int> initTilePos = new List<Vector3Int>();

        Vector3Int origin = new Vector3Int(Random.Range(boundary, roomSize.x - boundary), Random.Range(boundary, roomSize.y - boundary), 0);

        floorWalkers.Add(new Walker(origin, new Vector3Int(boundary, boundary, 0), new Vector3Int(roomSize.x - boundary, roomSize.y - boundary, 0)));
        initTilePos.Add(origin);
        randomFill = Random.Range(0.5f, 0.75f);
        int tilesToAdd = Mathf.FloorToInt(randomFill * (roomSize.x - (2 * boundary)) * (roomSize.y - (2 * boundary)));
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
                        RoomGrid[temp.x, temp.y] = TileType.floor;
                        currNumTiles++;
                    }
                }
            }
        }
        return initTilePos;
    }

    public List<Vector3Int> CreateWalls()
    {
        List<Vector3Int> initWallPos = new List<Vector3Int>();
        int prevBound = boundary;
        if (boundary <= 0)
        {
            boundary = 1;
        }
        for (int w = boundary; w < RoomGrid.GetLength(0) - boundary; w++)
        {
            for (int h = boundary; h < RoomGrid.GetLength(1) - boundary; h++)
            {
                if (RoomGrid[w, h] == TileType.floor)
                {
                    for (int n = 0; n < 8; n++)
                    {
                        switch (n)
                        {
                            case 0: // NW
                                if (RoomGrid[w - 1, h - 1] == TileType.empty )
                                {
                                    RoomGrid[w - 1, h - 1] = TileType.wall;
                                    initWallPos.Add(new Vector3Int(w - 1, h - 1, 0));
                                }
                                break;
                            case 1: // N
                                if (RoomGrid[w, h - 1] == TileType.empty)
                                {
                                    RoomGrid[w, h - 1] = TileType.wall;
                                    initWallPos.Add(new Vector3Int(w, h - 1, 0));
                                }
                                break;
                            case 2: // NE
                                if (RoomGrid[w + 1, h - 1] == TileType.empty)
                                {
                                    RoomGrid[w + 1, h - 1] = TileType.wall;
                                    initWallPos.Add(new Vector3Int(w + 1, h - 1, 0));
                                }
                                break;
                            case 3: // W
                                if (RoomGrid[w - 1, h] == TileType.empty)
                                {
                                    RoomGrid[w - 1, h] = TileType.wall;
                                    initWallPos.Add(new Vector3Int(w - 1, h, 0));
                                }
                                break;
                            case 4: // E
                                if (RoomGrid[w + 1, h] == TileType.empty)
                                {
                                    RoomGrid[w + 1, h] = TileType.wall;
                                    initWallPos.Add(new Vector3Int(w + 1, h, 0));
                                }
                                break;
                            case 5: // SW
                                if (RoomGrid[w - 1, h + 1] == TileType.empty)
                                {
                                    RoomGrid[w - 1, h + 1] = TileType.wall;
                                    initWallPos.Add(new Vector3Int(w - 1, h + 1, 0));
                                }
                                break;
                            case 6: // S
                                if (RoomGrid[w, h + 1] == TileType.empty)
                                {
                                    RoomGrid[w, h + 1] = TileType.wall;
                                    initWallPos.Add(new Vector3Int(w, h + 1, 0));
                                }
                                break;
                            default: // SE
                                if (RoomGrid[w + 1, h + 1] == TileType.empty)
                                {
                                    RoomGrid[w + 1, h + 1] = TileType.wall;
                                    initWallPos.Add(new Vector3Int(w + 1, h + 1, 0));
                                }
                                break;
                        }
                    }
                }
            }
        }
        if (prevBound != boundary)
        {
            boundary = prevBound;
        }
        return initWallPos;
    }

    public void CreateDoors()
    {
        int door = 0;
        foreach (bool d in doorways)
        {
            if (d)
            {
                Vector3Int leftBound = new Vector3Int(-1, -1, 0), rightBound = new Vector3Int(-1, -1, 0);
                switch (door)
                {
                    case 0: // top
                        for (int x = 0; x < RoomGrid.GetLength(0); x++)
                        {
                            for (int y = 0; y < RoomGrid.GetLength(1); y++)
                            {
                                if (RoomGrid[x, y] == TileType.wall && leftBound.x == -1)
                                {
                                    leftBound.x = x;
                                    leftBound.y = y;
                                }
                                if (RoomGrid[RoomGrid.GetLength(0) - x, y] == TileType.wall && rightBound.x == -1)
                                {
                                    rightBound.x = RoomGrid.GetLength(0) - x;
                                    rightBound.y = y;
                                }
                            }
                        }
                        if ((leftBound.x + 1) < rightBound.x)
                        {
                            int StartDoor = Random.Range(leftBound.x + 1, rightBound.x);

                        }
                        break;
                    case 1: // right
                        break;
                    case 2: // bottom
                        break;
                    case 3: //left
                        break;
                    default:
                        break;

                }
            }
            door++;
        } 
    }
}




