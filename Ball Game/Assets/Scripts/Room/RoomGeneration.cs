using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    public int numRooms;
    public Tilemap tilemap;
    public Tile floor;
    public Vector3Int totalRoomNum, roomSize;
    public GameObject PlayerObject;

    private int currRooms;
    private bool full, created;
    private Room[,] rooms;
    private Vector3Int startPos, gridSize;
    private List<Walker> walkers;
    private List<Vector3Int> initRooms; //roomPosToInit

    private void Start()
    {
        //init room array and starting position
        if (tilemap == null)
        {
            tilemap = new Tilemap();
        }
        if (totalRoomNum == null)
        {
            totalRoomNum = new Vector3Int(4, 4, 0);
        }
        if (roomSize == null)
        {
            roomSize = new Vector3Int(16, 16, 0);
        }
        gridSize = new Vector3Int(totalRoomNum.x * roomSize.x, totalRoomNum.y * roomSize.y, 0);
        tilemap.size = gridSize;
        
        rooms = new Room[totalRoomNum.x, totalRoomNum.y];
        startPos = new Vector3Int(Random.Range(0, totalRoomNum.x), Random.Range(0, totalRoomNum.y), 0);

        //branch and fill up the rest of the array with rooms
        if (numRooms > (totalRoomNum.x * totalRoomNum.y))
        {
            numRooms = (totalRoomNum.x * totalRoomNum.y);
        }

        full = false;
        created = false;

        walkers = new List<Walker>();
        //roomPosToInit = new List<Vector3Int>();
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
                walkers.Add(new Walker(startPos, new Vector3Int(0, 0, 0), totalRoomNum));
            }
        }
        else if(!created)
        {
            CreateRooms();
        }
    }

    public void AddRoom()
    {
        Debug.Log("Number of Rooms: " + currRooms);
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
                    Debug.Log("Passing by");
                }
                else
                {
                    initRooms.Add(temp);
                    currRooms++;
                }
            }
        }
        return;
    }

    public void CreateRooms()
    {
        foreach (Vector3Int v3i in initRooms)
        {
            Debug.Log("v3i: " + v3i);
            Debug.Log("v3i * roomsize: " + (v3i * roomSize));
            Room tempRoom = new Room(roomSize.x, roomSize.y);
            List<Vector3Int> tilesToAdd = tempRoom.CreateRoom(roomSize, 1);
            foreach (Vector3Int tta in tilesToAdd)
            {
                tilemap.SetTile(new Vector3Int(((v3i.x * roomSize.x) + tta.x), ((v3i.y * roomSize.y) + tta.y), 0), floor);
            }
        }
        created = true;
    }
}


public class Walker
{
    public Vector3Int pos, prevPos, lowerBounds, upperBounds;
    public int direction;

    public Walker()
    {
        pos = new Vector3Int(0, 0, 0);
        lowerBounds = new Vector3Int(0, 0, 0);
        upperBounds = new Vector3Int(8, 8, 0);
        prevPos = pos;
        direction = -1;
    }

    public Walker(Vector3Int sp, Vector3Int l, Vector3Int u)
    {
        pos = sp;
        lowerBounds = l;
        upperBounds = u;
        prevPos = pos;
        direction = -1;
    }

    public Walker(int x, int y, int lowX, int lowY, int upX, int upY)
    {
        pos = new Vector3Int(x, y, 0);
        lowerBounds = new Vector3Int(lowX, lowY, 0);
        upperBounds = new Vector3Int(upX, upY, 0);
        prevPos = pos;
        direction = -1;
    }

    public Vector3Int Step()
    {
        prevPos = pos;
        int option = Random.Range(0, 4);
        direction = option;
        switch (option)
        {
            case 0: // up
                pos.y--;
                if (pos.y < lowerBounds.y) // if pos is out of bounds
                {
                    pos.y++; // reset pos
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
                pos.y++;
                if (pos.y >= upperBounds.y) // if pos is out of bounds
                {
                    pos.y--; // reset pos
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
                direction = -1;
                break;
        }
        return pos;
    }

}