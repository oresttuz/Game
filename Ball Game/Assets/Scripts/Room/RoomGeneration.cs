using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class RoomGeneration : MonoBehaviour
{
    /*
     * Phase 1:
        Create Room Layout
        Create Individual rooms
        Connect Rooms

     * Phase 2:
        Update individual rooms to have more diverse layouts (Nuclear Throne)
        Determine where the doorways are
        Create the doorways
        Connect rooms using custom hallways
        (Optional) alter rooms to fit a theme or playstyle

     * Phase 3:
        Assign Mid Room
        Create boundraies for available positions/indexies for rooms.
        Create new room layout
        Assign Boss Room
        Create Individual rooms
        Connect rooms
        Assign Boss Room (Will Probably be a prefab)
    */

    public int numRooms, border;
    public Tilemap floorMap, wallMap;
    public Tile floor, wall, door, hallway;
    public Vector3Int totalRoomNum, roomSize;
    public GameObject PlayerObject;

    public TextMeshProUGUI pfTMP;
    public RectTransform VectorPanel;

    private int currRooms;
    private bool full, created;
    private Room[,] rooms;
    private Vector3Int startPos, gridSize;
    private List<Walker> walkers;
    private List<Vector3Int> initRooms; //roomPosToInit

    private void Start()
    {
        //init room array and starting position
        if (floorMap == null)
        {
            floorMap = new Tilemap();
        }
        if (wallMap == null)
        {
            wallMap = new Tilemap();
        }
        if (totalRoomNum == null)
        {
            totalRoomNum = new Vector3Int(4, 4, 0);
        }
        if (roomSize == null)
        {
            roomSize = new Vector3Int(16, 16, 0);
        }
        if (border < 4)
        {
            border = 4;
        }

        gridSize = new Vector3Int(totalRoomNum.x * roomSize.x, totalRoomNum.y * roomSize.y, 0);
        floorMap.size = gridSize;
        wallMap.size = gridSize;

        rooms = new Room[totalRoomNum.x, totalRoomNum.y];
        startPos = new Vector3Int(Random.Range(0, totalRoomNum.x), Random.Range(0, totalRoomNum.y), 0);
        rooms[startPos.x, startPos.y] = new Room(border, roomSize);

        //branch and fill up the rest of the array with rooms
        if (numRooms > (totalRoomNum.x * totalRoomNum.y))
        {
            numRooms = (totalRoomNum.x * totalRoomNum.y);
        }

        full = false;
        created = false;

        walkers = new List<Walker>();
        initRooms = new List<Vector3Int>();
        walkers.Add(new Walker(startPos, new Vector3Int(0, 0, 0), totalRoomNum)); // first walker

        //init first room
        PlayerObject.transform.position = startPos * roomSize;
        initRooms.Add(startPos);

        currRooms++;
    }

    private void FixedUpdate()
    {
        if (!full)
        {
            AddRoom();
            bool AddWalker = (Random.Range(0.0f, 1.0f) >= 0.5f);
            if (AddWalker)
            {
                //walkers.Add(new Walker(startPos, new Vector3Int(0, 0, 0), totalRoomNum));
            }
        }
        else if(!created)
        {
            Pathing();
            CreateRooms();
            CreateHallways();
        }
    }

    public void AddRoom()
    {
        Vector3Int temp;
        foreach (Walker w in walkers)
        {
            if (currRooms >= numRooms)
            {
                Debug.Log("Full");
                full = true;
                return;
            }
            temp = w.Step();
            if (temp != null)
            {
                if (initRooms.Contains(temp))
                {
                    //might add something here later
                }
                else
                {
                    rooms[temp.x, temp.y] = new Room(border, roomSize);
                    initRooms.Add(temp);
                    currRooms++;
                }
            }
        }
        return;
    }

    public void Pathing()
    {
        for(int x = 0; x < totalRoomNum.x; x++)
        {
            for (int y = 0; y < totalRoomNum.y; y++)
            {
                if (rooms[x, y] != null)
                {
                    if (y < totalRoomNum.y - 1) // UP
                    {
                        if (rooms[x, y + 1] != null)
                        {
                            rooms[x, y].Opening |= Direction.Up;
                        }
                    }
                    if (x < totalRoomNum.x - 1) // RIGHT
                    {
                        if (rooms[x + 1, y] != null)
                        {
                            rooms[x, y].Opening |= Direction.Right;
                        }
                    }
                    if (y >= 1) // DOWN
                    {
                        if (rooms[x, y - 1] != null)
                        {
                            rooms[x, y].Opening |= Direction.Down;
                        }
                    }
                    if (x >= 1) // LEFT
                    {
                        if (rooms[x - 1, y] != null)
                        {
                            rooms[x, y].Opening |= Direction.Left;
                        }
                    }
                }
            }
        }
    }

    public void CreateRooms()
    {
        foreach (Vector3Int v3i in initRooms)
        {
            Debug.Log(v3i + ": " + rooms[v3i.x, v3i.y].Opening );
            TextMeshProUGUI vectorPos = Instantiate(pfTMP, VectorPanel);
            vectorPos.transform.localPosition = (v3i * roomSize) + (new Vector3(roomSize.x * 0.5f, roomSize.y * 0.5f, 0f));
            vectorPos.text = "" + v3i;

            List<Vector3Int> floorsToAdd = rooms[v3i.x, v3i.y].CreateFloors(roomSize);
            foreach (Vector3Int fta in floorsToAdd)
            {
                floorMap.SetTile(new Vector3Int(((v3i.x * roomSize.x) + fta.x), ((v3i.y * roomSize.y) + fta.y), 0), floor);
            }
            List<Vector3Int> wallsToAdd = rooms[v3i.x, v3i.y].CreateWalls(1==1);
            foreach (Vector3Int wta in wallsToAdd)
            {
                wallMap.SetTile(new Vector3Int(((v3i.x * roomSize.x) + wta.x), ((v3i.y * roomSize.y) + wta.y), 0), wall);
            }
            DoorList doorsAndMoreToAdd = rooms[v3i.x, v3i.y].CreateAllDoors();
            foreach (Vector3Int f in doorsAndMoreToAdd.floor)
            {
                wallMap.SetTile(new Vector3Int(((v3i.x * roomSize.x) + f.x), ((v3i.y * roomSize.y) + f.y), 0), null);
                floorMap.SetTile(new Vector3Int(((v3i.x * roomSize.x) + f.x), ((v3i.y * roomSize.y) + f.y), 0), floor);
            }
            foreach (Vector3Int w in doorsAndMoreToAdd.wall)
            {
                wallMap.SetTile(new Vector3Int(((v3i.x * roomSize.x) + w.x), ((v3i.y * roomSize.y) + w.y), 0), wall);
            }
            foreach (Vector3Int d in doorsAndMoreToAdd.door)
            {
                wallMap.SetTile(new Vector3Int(((v3i.x * roomSize.x) + d.x), ((v3i.y * roomSize.y) + d.y), 0), door);
            }
        }
        created = true;
    }

    public void CreateHallways()
    {
        List<Vector3Int> allHallwayTilePositions = new List<Vector3Int>();
        for (int x = 0; x < totalRoomNum.x; x++)
        {
            for (int y = 0; y < totalRoomNum.y; y++)
            {
                if (rooms[x, y] != null)
                {
                    if (!rooms[x, y].Equals(Direction.None))
                    {
                        if (rooms[x, y].Opening.HasFlag(Direction.Up) && !(rooms[x, y].HallwayMade.HasFlag(Direction.Up)) && y < totalRoomNum.y - 1)
                        {
                            if (rooms[x, y + 1] != null)
                            {
                                if (rooms[x, y].doors[0] != null && rooms[x, y + 1].doors[2] != null)
                                {
                                    allHallwayTilePositions.AddRange(RoomScale(rooms[x, y].CreateHallwayUp(rooms[x, y + 1].doors[2].SouthWestPos.x), new Vector3Int(x, y, 0)));
                                    allHallwayTilePositions.AddRange(RoomScale(rooms[x, y + 1].CreateHallwayDown(rooms[x, y].doors[0].SouthWestPos.x), new Vector3Int(x, y + 1, 0)));
                                }
                            }
                        }
                        if (rooms[x, y].Opening.HasFlag(Direction.Right) && !(rooms[x, y].HallwayMade.HasFlag(Direction.Right)) && x < totalRoomNum.x - 1)
                        {
                            if (rooms[x + 1, y] != null)
                            {
                                if (rooms[x, y].doors[1] != null && rooms[x + 1, y].doors[3] != null)
                                {
                                    allHallwayTilePositions.AddRange(RoomScale(rooms[x, y].CreateHallwayRight(rooms[x + 1, y].doors[3].SouthWestPos.y), new Vector3Int(x, y, 0)));
                                    allHallwayTilePositions.AddRange(RoomScale(rooms[x + 1, y].CreateHallwayLeft(rooms[x, y].doors[1].SouthWestPos.y), new Vector3Int(x + 1, y, 0)));
                                }
                            }
                        }
                        if (rooms[x, y].Opening.HasFlag(Direction.Down) && !(rooms[x, y].HallwayMade.HasFlag(Direction.Down)) && y >= 1)
                        {
                            if (rooms[x, y - 1] != null)
                            {
                                if (rooms[x, y].doors[2] != null && rooms[x, y - 1].doors[0] != null)
                                {
                                    allHallwayTilePositions.AddRange(RoomScale(rooms[x, y].CreateHallwayDown(rooms[x, y - 1].doors[0].SouthWestPos.x), new Vector3Int(x, y, 0)));
                                    allHallwayTilePositions.AddRange(RoomScale(rooms[x, y - 1].CreateHallwayUp(rooms[x, y].doors[2].SouthWestPos.x), new Vector3Int(x, y - 1, 0)));
                                }
                            }
                        }
                        if (rooms[x, y].Opening.HasFlag(Direction.Left) && !(rooms[x, y].HallwayMade.HasFlag(Direction.Left)) && x >= 1)
                        {
                            if (rooms[x - 1, y] != null)
                            {
                                if (rooms[x, y].doors[3] != null && rooms[x - 1, y].doors[1] != null)
                                {
                                    allHallwayTilePositions.AddRange(RoomScale(rooms[x, y].CreateHallwayLeft(rooms[x - 1, y].doors[1].SouthWestPos.x), new Vector3Int(x, y, 0)));
                                    allHallwayTilePositions.AddRange(RoomScale(rooms[x - 1, y].CreateHallwayRight(rooms[x, y].doors[3].SouthWestPos.x), new Vector3Int(x - 1, y, 0)));
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("Room (" + x + ", " + y + ") had no hallways to generate");
                    }
                }
            }
        }
        foreach (Vector3Int hwTile in allHallwayTilePositions)
        {
            floorMap.SetTile(hwTile, hallway);
        }
        List<Vector3Int> wallTiles = new List<Vector3Int>();
        foreach (Vector3Int roomVector in initRooms)
        {
            wallTiles.AddRange(RoomScale(rooms[roomVector.x, roomVector.y].CreateWalls(false), roomVector));
        }
        foreach (Vector3Int wt in wallTiles)
        {
            wallMap.SetTile(wt, wall);
        }
    }

    public List<Vector3Int> RoomScale(List<Vector3Int> vects, Vector3Int scale)
    {
        List<Vector3Int> scaledVects = new List<Vector3Int>(); 
        foreach (Vector3Int v in vects)
        {
            scaledVects.Add(new Vector3Int(((scale.x * roomSize.x) + v.x), ((scale.y * roomSize.y) + v.y), 0));
        }
        return scaledVects;
    }
}


public class Walker
{
    public Vector3Int pos, prevPos, lowerBounds, upperBounds;

    public Walker()
    {
        pos = new Vector3Int(0, 0, 0);
        lowerBounds = new Vector3Int(0, 0, 0);
        upperBounds = new Vector3Int(8, 8, 0);
        prevPos = pos;
    }

    public Walker(Vector3Int sp, Vector3Int l, Vector3Int u)
    {
        pos = sp;
        lowerBounds = l;
        upperBounds = u;
        prevPos = pos;
    }

    public Walker(int x, int y, int lowX, int lowY, int upX, int upY)
    {
        pos = new Vector3Int(x, y, 0);
        lowerBounds = new Vector3Int(lowX, lowY, 0);
        upperBounds = new Vector3Int(upX, upY, 0);
        prevPos = pos;
    }

    public Vector3Int Step()
    {
        prevPos = pos;
        int option = Random.Range(0, 4);
        switch (option)
        {
            case 0: // up
                pos.y++;
                if (pos.y >= upperBounds.y) // if pos is out of bounds
                {
                    pos.y--; // reset pos
                    pos = Step(); // try again
                }
                break;
            case 1: // right
                pos.x++;
                if (pos.x >= upperBounds.x) // if pos is out of bounds
                {
                    pos.x--; // reset pos
                    pos = Step(); // try again
                }
                break;
            case 2: // down
                pos.y--;
                if (pos.y < lowerBounds.y) // if pos is out of bounds
                {
                    pos.y++; // reset pos
                    pos = Step(); // try again
                }
                break;
            case 3: // left
                pos.x--;
                if (pos.x < lowerBounds.x) // if pos is out of bounds
                {
                    pos.x++; // reset pos
                    pos = Step(); // try again
                }
                break;
            default: //invalid option
                break;
        }
        return pos;
    }

    public Vector3Int ControlledStep(int option)
    {
        Vector3Int temp = prevPos;
        prevPos = pos;
        switch (option)
        {
            case 0: // up
                pos.y++;
                if (pos.y >= upperBounds.y) // if pos is out of bounds
                {
                    pos.y--; // reset pos
                    prevPos = temp;
                    return temp = new Vector3Int(-1, -1, -1);
                }
                break;
            case 1: // right
                pos.x++;
                if (pos.x >= upperBounds.x) // if pos is out of bounds
                {
                    pos.x--; // reset pos
                    prevPos = temp;
                    return temp = new Vector3Int(-1, -1, -1);
                }
                break;
            case 2: // down
                pos.y--;
                if (pos.y < lowerBounds.y) // if pos is out of bounds
                {
                    pos.y++; // reset pos
                    prevPos = temp;
                    return temp = new Vector3Int(-1, -1, -1);
                }
                break;
            case 3: // left
                pos.x--;
                if (pos.x < lowerBounds.x) // if pos is out of bounds
                {
                    pos.x++; // reset pos
                    prevPos = temp;
                    return temp = new Vector3Int(-1, -1, -1);
                }
                break;
            default: //invalid option
                Debug.Log("Invalid Option for Controlled Step");
                return temp = new Vector3Int(-1, -1, -1);
        }
        return pos;
    }

    public string PrintWalker() { return "Pos: " + pos + ", Lower Bounds: " + lowerBounds + ", Upper Bounds:" + upperBounds + ", Previous Positon: " + prevPos; }
}