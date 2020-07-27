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
                    initWallPos.AddRange(FindNeighborsToWall(w, h));
                }
            }
        }
        if (prevBound != boundary)
        {
            boundary = prevBound;
        }
        return initWallPos;
    }

    public DoorList CreateDoors()
    {
        int door = 0;
        DoorList dl = new DoorList();
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
                                if (RoomGrid[RoomGrid.GetLength(0) - 1 - x, y] == TileType.wall && rightBound.x == -1)
                                {
                                    rightBound.x = RoomGrid.GetLength(0) - x;
                                    rightBound.y = y;
                                }
                            }
                        }
                        if ((leftBound.x + 1) < rightBound.x)
                        {
                            int doorDiff = 1;
                            int side = 0; // left = -1, right = 1
                            int StartDoor = Random.Range(leftBound.x + 1, rightBound.x);
                            int leftDis = StartDoor - leftBound.x;
                            int rightDis = rightBound.x - StartDoor;
                            if (leftDis > rightDis)
                            {
                                doorDiff = Mathf.FloorToInt(leftDis/rightDis);
                                side = -1;
                            }
                            else
                            {
                                doorDiff = Mathf.FloorToInt(rightDis / leftDis);
                                side = 1;
                            }
                            Walker left = new Walker(StartDoor, 0, 0, 0, RoomGrid.GetLength(0), RoomGrid.GetLength(1));
                            Walker right = new Walker(StartDoor, 0, 0, 0, RoomGrid.GetLength(0), RoomGrid.GetLength(1));
                            bool DoorFound = false;
                            int ratioCount = 0;
                            Vector3Int bad = new Vector3Int(-1, -1, -1);
                            while (!DoorFound)
                            {
                                Vector3Int leftTemp = left.ControlledStep(3);
                                Vector3Int rightTemp = right.ControlledStep(1);
                                if (leftTemp == bad)
                                {
                                    left.pos = new Vector3Int(StartDoor, (left.pos.y + 1), left.pos.z); // bad if y is > room
                                    left.prevPos = left.pos;
                                }
                                else
                                {
                                    if (left.pos != left.prevPos && left.pos.y == left.prevPos.y)
                                    {
                                        if (RoomGrid[left.pos.x, left.pos.y] == TileType.wall && RoomGrid[left.prevPos.x, left.prevPos.y] == TileType.wall)
                                        {
                                            DoorFound = true;
                                            if (RoomGrid[left.pos.x, left.pos.y + 1] != TileType.floor || RoomGrid[left.prevPos.x, left.prevPos.y + 1] != TileType.floor) // bad if y is > room
                                            {
                                                RoomGrid[left.pos.x, left.pos.y + 1] = TileType.floor;
                                                RoomGrid[left.prevPos.x, left.prevPos.y + 1] = TileType.floor;
                                                dl.floor.Add(new Vector3Int(left.pos.x, left.pos.y + 1, left.pos.z));
                                                dl.floor.Add(new Vector3Int(left.prevPos.x, left.prevPos.y + 1, left.prevPos.z));
                                                dl.wall.AddRange(FindNeighborsToWall(left.pos.x, left.pos.y + 1));
                                                dl.wall.AddRange(FindNeighborsToWall(left.prevPos.x, left.prevPos.y + 1));
                                            }
                                            RoomGrid[left.pos.x, left.pos.y] = TileType.door;
                                            RoomGrid[left.prevPos.x, left.prevPos.y] = TileType.door;
                                            dl.door.Add(left.pos);
                                            dl.door.Add(left.prevPos);
                                        }
                                    }
                                }
                                if (rightTemp == bad)
                                {
                                    right.pos = new Vector3Int(StartDoor, (right.pos.y + 1), right.pos.z); // bad if y is > room
                                    right.prevPos = right.pos;
                                }
                                else
                                {
                                    if (right.pos != right.prevPos && right.pos.y == right.prevPos.y && !DoorFound)
                                    {
                                        if (RoomGrid[right.pos.x, right.pos.y] == TileType.wall && RoomGrid[right.prevPos.x, right.prevPos.y] == TileType.wall)
                                        {
                                            DoorFound = true;
                                            if (RoomGrid[right.pos.x, right.pos.y + 1] != TileType.floor || RoomGrid[right.prevPos.x, right.prevPos.y + 1] != TileType.floor) // bad if y is > room
                                            {
                                                RoomGrid[right.pos.x, right.pos.y + 1] = TileType.floor;
                                                RoomGrid[right.prevPos.x, right.prevPos.y + 1] = TileType.floor;
                                                dl.floor.Add(new Vector3Int(right.pos.x, right.pos.y + 1, right.pos.z));
                                                dl.floor.Add(new Vector3Int(right.prevPos.x, right.prevPos.y + 1, right.prevPos.z));
                                                dl.wall.AddRange(FindNeighborsToWall(right.pos.x, right.pos.y + 1));
                                                dl.wall.AddRange(FindNeighborsToWall(right.prevPos.x, right.prevPos.y + 1));
                                            }
                                            RoomGrid[right.pos.x, right.pos.y] = TileType.door;
                                            RoomGrid[right.prevPos.x, right.prevPos.y] = TileType.door;
                                            dl.door.Add(right.pos);
                                            dl.door.Add(right.prevPos);
                                        }
                                    }
                                }

                            }
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
        return dl;
    }

    public List<Vector3Int> FindNeighborsToWall(int x, int y)
    {
        List<Vector3Int> temp = new List<Vector3Int>();
        for (int n = 0; n < 8; n++)
        {
            switch (n)
            {
                case 0: // NW
                    if (RoomGrid[x - 1, y - 1] == TileType.empty)
                    {
                        RoomGrid[x - 1, y - 1] = TileType.wall;
                        temp.Add(new Vector3Int(x - 1, y - 1, 0));
                    }
                    break;
                case 1: // N
                    if (RoomGrid[x, y - 1] == TileType.empty)
                    {
                        RoomGrid[x, y - 1] = TileType.wall;
                        temp.Add(new Vector3Int(x, y - 1, 0));
                    }
                    break;
                case 2: // NE
                    if (RoomGrid[x + 1, y - 1] == TileType.empty)
                    {
                        RoomGrid[x + 1, y - 1] = TileType.wall;
                        temp.Add(new Vector3Int(x + 1, y - 1, 0));
                    }
                    break;
                case 3: // W
                    if (RoomGrid[x - 1, y] == TileType.empty)
                    {
                        RoomGrid[x - 1, y] = TileType.wall;
                        temp.Add(new Vector3Int(x - 1, y, 0));
                    }
                    break;
                case 4: // E
                    if (RoomGrid[x + 1, y] == TileType.empty)
                    {
                        RoomGrid[x + 1, y] = TileType.wall;
                        temp.Add(new Vector3Int(x + 1, y, 0));
                    }
                    break;
                case 5: // SW
                    if (RoomGrid[x - 1, y + 1] == TileType.empty)
                    {
                        RoomGrid[x - 1, y + 1] = TileType.wall;
                        temp.Add(new Vector3Int(x - 1, y + 1, 0));
                    }
                    break;
                case 6: // S
                    if (RoomGrid[x, y + 1] == TileType.empty)
                    {
                        RoomGrid[x, y + 1] = TileType.wall;
                        temp.Add(new Vector3Int(x, y + 1, 0));
                    }
                    break;
                default: // SE
                    if (RoomGrid[x + 1, y + 1] == TileType.empty)
                    {
                        RoomGrid[x + 1, y + 1] = TileType.wall;
                        temp.Add(new Vector3Int(x + 1, y + 1, 0));
                    }
                    break;
            }
        }
        return temp;

    }
}

public class DoorList
{
    public List<Vector3Int> floor;
    public List<Vector3Int> wall;
    public List<Vector3Int> door;

    public DoorList()
    {
        floor = new List<Vector3Int>();
        wall = new List<Vector3Int>();
        door = new List<Vector3Int>();
    }

}



