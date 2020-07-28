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

    public string PrintDoorways()
    {
        string doors = "No doors";
        int count = 0;
        foreach (bool d in doorways)
        {
            if (d)
            {
                if (doors == "No doors")
                {
                    doors = "Door(s): ";
                }
                else
                {
                    doors += ", ";
                }
                switch (count)
                {
                    case 0:
                        doors += "Up";
                        break;
                    case 1:
                        doors += "Left";
                        break;
                    case 2:
                        doors += "Down";
                        break;
                    case 3:
                        doors += "Right";
                        break;
                    default:
                        break;
                }
            }
            count++;
        }
        return doors;
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
        int door = 0, iterations = 0, maxIt = 9999;
        DoorList dl = new DoorList();
        foreach (bool d in doorways)
        {
            if (d)
            {
                Vector3Int lowBound = new Vector3Int(-1, -1, 0), highBound = new Vector3Int(-1, -1, 0);
                switch (door)
                {
                    case 0: // top //updated
                        for (int x = 0; x < RoomGrid.GetLength(0); x++)
                        {
                            for (int y = RoomGrid.GetLength(1) - 1; y >= 0 ; y--)
                            {
                                if (RoomGrid[x, y] == TileType.wall && lowBound.x == -1)
                                {
                                    lowBound.x = x;
                                    lowBound.y = y;
                                }
                                if (RoomGrid[RoomGrid.GetLength(0) - 1 - x, y] == TileType.wall && highBound.x == -1)
                                {
                                    highBound.x = RoomGrid.GetLength(0) - x;
                                    highBound.y = y;
                                }
                            }
                        }
                        if ((lowBound.x + 1) < highBound.x)
                        {
                            int StartDoor = Random.Range(lowBound.x + 1, highBound.x);
                            Walker left = new Walker(StartDoor, RoomGrid.GetLength(1) - 1, 0, 0, RoomGrid.GetLength(0), RoomGrid.GetLength(1));
                            Walker right = new Walker(StartDoor, RoomGrid.GetLength(1) - 1, 0, 0, RoomGrid.GetLength(0), RoomGrid.GetLength(1));
                            bool DoorFound = false;
                            Vector3Int bad = new Vector3Int(-1, -1, -1);
                            iterations = 0;
                            while (!DoorFound && iterations < maxIt)
                            {
                                Vector3Int leftTemp = left.ControlledStep(3);
                                Vector3Int rightTemp = right.ControlledStep(1);
                                if (leftTemp == bad)
                                {
                                    left.pos = new Vector3Int(StartDoor, (left.pos.y - 1), left.pos.z); 
                                    left.prevPos = left.pos;
                                }
                                else
                                {
                                    if (left.pos != left.prevPos && left.pos.y == left.prevPos.y)
                                    {
                                        if (RoomGrid[left.pos.x, left.pos.y] == TileType.wall && RoomGrid[left.prevPos.x, left.prevPos.y] == TileType.wall)
                                        {
                                            DoorFound = true;
                                            if (RoomGrid[left.pos.x, left.pos.y - 1] != TileType.floor || RoomGrid[left.prevPos.x, left.prevPos.y - 1] != TileType.floor)
                                            {
                                                RoomGrid[left.pos.x, left.pos.y - 1] = TileType.floor;
                                                RoomGrid[left.prevPos.x, left.prevPos.y - 1] = TileType.floor;
                                                dl.floor.Add(new Vector3Int(left.pos.x, left.pos.y - 1, left.pos.z));
                                                dl.floor.Add(new Vector3Int(left.prevPos.x, left.prevPos.y - 1, left.prevPos.z));
                                                dl.wall.AddRange(FindNeighborsToWall(left.pos.x, left.pos.y - 1));
                                                dl.wall.AddRange(FindNeighborsToWall(left.prevPos.x, left.prevPos.y - 1));
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
                                    right.pos = new Vector3Int(StartDoor, (right.pos.y - 1), right.pos.z); 
                                    right.prevPos = right.pos;
                                }
                                else
                                {
                                    if (right.pos != right.prevPos && right.pos.y == right.prevPos.y && !DoorFound)
                                    {
                                        if (RoomGrid[right.pos.x, right.pos.y] == TileType.wall && RoomGrid[right.prevPos.x, right.prevPos.y] == TileType.wall)
                                        {
                                            DoorFound = true;
                                            if (RoomGrid[right.pos.x, right.pos.y - 1] != TileType.floor || RoomGrid[right.prevPos.x, right.prevPos.y - 1] != TileType.floor) 
                                            {
                                                RoomGrid[right.pos.x, right.pos.y - 1] = TileType.floor;
                                                RoomGrid[right.prevPos.x, right.prevPos.y - 1] = TileType.floor;
                                                dl.floor.Add(new Vector3Int(right.pos.x, right.pos.y - 1, right.pos.z));
                                                dl.floor.Add(new Vector3Int(right.prevPos.x, right.prevPos.y - 1, right.prevPos.z));
                                                dl.wall.AddRange(FindNeighborsToWall(right.pos.x, right.pos.y - 1));
                                                dl.wall.AddRange(FindNeighborsToWall(right.prevPos.x, right.prevPos.y - 1));
                                            }
                                            RoomGrid[right.pos.x, right.pos.y] = TileType.door;
                                            RoomGrid[right.prevPos.x, right.prevPos.y] = TileType.door;
                                            dl.door.Add(right.pos);
                                            dl.door.Add(right.prevPos);
                                        }
                                    }
                                }
                            }
                            //iterations++;
                        }
                        if (iterations >= maxIt)
                        {
                            Debug.Log("Case 0 Failed: Hit Max Iterations (" + iterations + ")");
                        }
                        break;
                    case 1: // right //needs to be changed
                        for (int y = 0; y < RoomGrid.GetLength(1); y++)
                        {
                            for (int x = RoomGrid.GetLength(0) - 1; x >= 0; x--)
                            {
                                if (RoomGrid[x, y] == TileType.wall && lowBound.y == -1)
                                {
                                    lowBound.x = x;
                                    lowBound.y = y;
                                }
                                if (RoomGrid[x, RoomGrid.GetLength(1) - 1 - y] == TileType.wall && highBound.y == -1)
                                {
                                    highBound.x = x;
                                    highBound.y = RoomGrid.GetLength(1) - y;
                                }
                            }
                        }
                        if ((lowBound.y + 1) < highBound.y)
                        {
                            int StartDoor = Random.Range(lowBound.y + 1, highBound.y);
                            Walker down = new Walker(RoomGrid.GetLength(0) - 1, StartDoor, 0, 0, RoomGrid.GetLength(0), RoomGrid.GetLength(1));
                            Walker up = new Walker(RoomGrid.GetLength(0) - 1, StartDoor, 0, 0, RoomGrid.GetLength(0), RoomGrid.GetLength(1));
                            bool DoorFound = false;
                            Vector3Int bad = new Vector3Int(-1, -1, -1);
                            iterations = 0;
                            while (!DoorFound && iterations < maxIt)
                            {
                                Vector3Int downTemp = down.ControlledStep(2);
                                Vector3Int upTemp = up.ControlledStep(0);
                                if (downTemp == bad)
                                {
                                    down.pos = new Vector3Int((down.pos.x - 1), StartDoor, down.pos.z); // bad if y is > room
                                    down.prevPos = down.pos;
                                }
                                else
                                {
                                    if (down.pos != down.prevPos && down.pos.x == down.prevPos.x)
                                    {
                                        if (RoomGrid[down.pos.x, down.pos.y] == TileType.wall && RoomGrid[down.prevPos.x, down.prevPos.y] == TileType.wall)
                                        {
                                            DoorFound = true;
                                            if (RoomGrid[down.pos.x - 1, down.pos.y] != TileType.floor || RoomGrid[down.prevPos.x - 1, down.prevPos.y] != TileType.floor) // bad if y is > room
                                            {
                                                RoomGrid[down.pos.x - 1, down.pos.y] = TileType.floor;
                                                RoomGrid[down.prevPos.x - 1, down.prevPos.y] = TileType.floor;
                                                dl.floor.Add(new Vector3Int(down.pos.x - 1, down.pos.y, down.pos.z));
                                                dl.floor.Add(new Vector3Int(down.prevPos.x - 1, down.prevPos.y, down.prevPos.z));
                                                dl.wall.AddRange(FindNeighborsToWall(down.pos.x - 1, down.pos.y));
                                                dl.wall.AddRange(FindNeighborsToWall(down.prevPos.x - 1, down.prevPos.y));
                                            }
                                            RoomGrid[down.pos.x, down.pos.y] = TileType.door;
                                            RoomGrid[down.prevPos.x, down.prevPos.y] = TileType.door;
                                            dl.door.Add(down.pos);
                                            dl.door.Add(down.prevPos);
                                        }
                                    }
                                }
                                if (upTemp == bad)
                                {
                                    up.pos = new Vector3Int((up.pos.x - 1), StartDoor, up.pos.z); // bad if y is > room
                                    up.prevPos = up.pos;
                                }
                                else
                                {
                                    if (up.pos != up.prevPos && up.pos.x == up.prevPos.x)
                                    {
                                        if (RoomGrid[up.pos.x, up.pos.y] == TileType.wall && RoomGrid[up.prevPos.x, up.prevPos.y] == TileType.wall)
                                        {
                                            DoorFound = true;
                                            if (RoomGrid[up.pos.x - 1, up.pos.y] != TileType.floor || RoomGrid[up.prevPos.x - 1, up.prevPos.y] != TileType.floor) // bad if y is > room
                                            {
                                                RoomGrid[up.pos.x - 1, up.pos.y] = TileType.floor;
                                                RoomGrid[up.prevPos.x - 1, up.prevPos.y] = TileType.floor;
                                                dl.floor.Add(new Vector3Int(up.pos.x - 1, up.pos.y, up.pos.z));
                                                dl.floor.Add(new Vector3Int(up.prevPos.x - 1, up.prevPos.y, up.prevPos.z));
                                                dl.wall.AddRange(FindNeighborsToWall(up.pos.x - 1, up.pos.y));
                                                dl.wall.AddRange(FindNeighborsToWall(up.prevPos.x - 1, up.prevPos.y));
                                            }
                                            RoomGrid[up.pos.x, up.pos.y] = TileType.door;
                                            RoomGrid[up.prevPos.x, up.prevPos.y] = TileType.door;
                                            dl.door.Add(up.pos);
                                            dl.door.Add(up.prevPos);
                                        }
                                    }
                                }
                                //iterations++;
                            }
                        }
                        if (iterations >= maxIt)
                        {
                            Debug.Log("Case 1 Failed: Hit Max Iterations (" + iterations + ")");
                        }
                        break;
                    case 2: // bottom //orig
                        for (int x = 0; x < RoomGrid.GetLength(0); x++)
                        {
                            for (int y = 0; y < RoomGrid.GetLength(1); y++)
                            {
                                if (RoomGrid[x, y] == TileType.wall && lowBound.x == -1)
                                {
                                    lowBound.x = x;
                                    lowBound.y = y;
                                }
                                if (RoomGrid[RoomGrid.GetLength(0) - 1 - x, y] == TileType.wall && highBound.x == -1)
                                {
                                    highBound.x = RoomGrid.GetLength(0) - x;
                                    highBound.y = y;
                                }
                            }
                        }
                        if ((lowBound.x + 1) < highBound.x)
                        {
                            int StartDoor = Random.Range(lowBound.x + 1, highBound.x);
                            Walker left = new Walker(StartDoor, 0, 0, 0, RoomGrid.GetLength(0), RoomGrid.GetLength(1));
                            Walker right = new Walker(StartDoor, 0, 0, 0, RoomGrid.GetLength(0), RoomGrid.GetLength(1));
                            bool DoorFound = false;
                            Vector3Int bad = new Vector3Int(-1, -1, -1);
                            iterations = 0;
                            while (!DoorFound && iterations < maxIt)
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
                                //iterations++;
                            }
                        }
                        if (iterations >= maxIt)
                        {
                            Debug.Log("Case 2 Failed: Hit Max Iterations (" + iterations + ")");
                        }
                        break;
                    case 3: //left //needs to be changed
                        for (int y = 0; y < RoomGrid.GetLength(1); y++)
                        {
                            for (int x = 0; x < RoomGrid.GetLength(0); x++)
                            {
                                if (RoomGrid[x, y] == TileType.wall && lowBound.y == -1)
                                {
                                    lowBound.x = x;
                                    lowBound.y = y;
                                }
                                if (RoomGrid[x, RoomGrid.GetLength(1) - 1 - y] == TileType.wall && highBound.y == -1)
                                {
                                    highBound.x = x;
                                    highBound.y = RoomGrid.GetLength(1) - y;
                                }
                            }
                        }
                        if ((lowBound.y + 1) < highBound.y)
                        {
                            int StartDoor = Random.Range(lowBound.y + 1, highBound.y);
                            Walker down = new Walker(0, StartDoor, 0, 0, RoomGrid.GetLength(0), RoomGrid.GetLength(1));
                            Walker up = new Walker(0, StartDoor, 0, 0, RoomGrid.GetLength(0), RoomGrid.GetLength(1));
                            bool DoorFound = false;
                            Vector3Int bad = new Vector3Int(-1, -1, -1);
                            iterations = 0;
                            while (!DoorFound && iterations < maxIt)
                            {
                                Vector3Int downTemp = down.ControlledStep(2);
                                Vector3Int upTemp = up.ControlledStep(0);
                                if (downTemp == bad)
                                {
                                    down.pos = new Vector3Int((down.pos.x + 1), StartDoor, down.pos.z); // bad if y is > room
                                    down.prevPos = down.pos;
                                }
                                else
                                {
                                    if (down.pos != down.prevPos && down.pos.x == down.prevPos.x)
                                    {
                                        if (RoomGrid[down.pos.x, down.pos.y] == TileType.wall && RoomGrid[down.prevPos.x, down.prevPos.y] == TileType.wall)
                                        {
                                            DoorFound = true;
                                            if (RoomGrid[down.pos.x + 1, down.pos.y] != TileType.floor || RoomGrid[down.prevPos.x + 1, down.prevPos.y] != TileType.floor) // bad if y is > room
                                            {
                                                RoomGrid[down.pos.x + 1, down.pos.y] = TileType.floor;
                                                RoomGrid[down.prevPos.x + 1, down.prevPos.y] = TileType.floor;
                                                dl.floor.Add(new Vector3Int(down.pos.x + 1, down.pos.y, down.pos.z));
                                                dl.floor.Add(new Vector3Int(down.prevPos.x + 1, down.prevPos.y, down.prevPos.z));
                                                dl.wall.AddRange(FindNeighborsToWall(down.pos.x + 1, down.pos.y));
                                                dl.wall.AddRange(FindNeighborsToWall(down.prevPos.x + 1, down.prevPos.y));
                                            }
                                            RoomGrid[down.pos.x, down.pos.y] = TileType.door;
                                            RoomGrid[down.prevPos.x, down.prevPos.y] = TileType.door;
                                            dl.door.Add(down.pos);
                                            dl.door.Add(down.prevPos);
                                        }
                                    }
                                }
                                if (upTemp == bad)
                                {
                                    up.pos = new Vector3Int((up.pos.x + 1), StartDoor, up.pos.z); // bad if y is > room
                                    up.prevPos = up.pos;
                                }
                                else
                                {
                                    if (up.pos != up.prevPos && up.pos.x == up.prevPos.x)
                                    {
                                        if (RoomGrid[up.pos.x, up.pos.y] == TileType.wall && RoomGrid[up.prevPos.x, up.prevPos.y] == TileType.wall)
                                        {
                                            DoorFound = true;
                                            if (RoomGrid[up.pos.x + 1, up.pos.y] != TileType.floor || RoomGrid[up.prevPos.x + 1, up.prevPos.y] != TileType.floor) // bad if y is > room
                                            {
                                                RoomGrid[up.pos.x + 1, up.pos.y] = TileType.floor;
                                                RoomGrid[up.prevPos.x + 1, up.prevPos.y] = TileType.floor;
                                                dl.floor.Add(new Vector3Int(up.pos.x + 1, up.pos.y, up.pos.z));
                                                dl.floor.Add(new Vector3Int(up.prevPos.x + 1, up.prevPos.y, up.prevPos.z));
                                                dl.wall.AddRange(FindNeighborsToWall(up.pos.x + 1, up.pos.y));
                                                dl.wall.AddRange(FindNeighborsToWall(up.prevPos.x + 1, up.prevPos.y));
                                            }
                                            RoomGrid[up.pos.x, up.pos.y] = TileType.door;
                                            RoomGrid[up.prevPos.x, up.prevPos.y] = TileType.door;
                                            dl.door.Add(up.pos);
                                            dl.door.Add(up.prevPos);
                                        }
                                    }
                                }
                                //iterations++;
                            }
                        }
                        if (iterations >= maxIt)
                        {
                            Debug.Log("Case 3 Failed: Hit Max Iterations (" + iterations + ")");
                        }
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

    public void AddDoorListRange(DoorList d)
    {
        floor.AddRange(d.floor);
        wall.AddRange(d.wall);
        door.AddRange(d.door);
        return;
    }
}



