using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGeneration : MonoBehaviour
{
    /*
     * Phase 1:
        Create Room Layout
        Create Individual rooms
        Connect Rooms

     * Phase 2:
        Assign Mid Room
        Create boundraies for available positions/indexies for rooms.
        Create new room layout
        Assign Boss Room
        Create Individual rooms
        Connect rooms
        Assign Boss Room (Will Probably be a prefab)

     * Phase 3:
        Update individual rooms to have more diverse layouts (Nuclear Throne)
        Determine where the doorways are
        Create the doorways
        Connect rooms using custom hallways
        (Optional) alter rooms to fit a theme or playstyle
    */

    private Room[,] rooms;
    public Vector2Int gridSize, startPos;
    public int numRooms;

    private List<Walker> walkers;
    private List<Vector2Int> roomPosToInit, initRooms;

    private void Start()
    {
        //init room array and starting position
        if (gridSize == null)
        {
            gridSize = new Vector2Int(8, 8);
        }
        rooms = new Room[gridSize.x, gridSize.y];
        if (startPos == null)
        {
            startPos = new Vector2Int(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
        }

        //branch and fill up the rest of the array with rooms
        if (numRooms > (gridSize.x * gridSize.x))
        {
            numRooms = (gridSize.x * gridSize.x);
        }
        walkers = new List<Walker>();
        roomPosToInit = new List<Vector2Int>();
        initRooms = new List<Vector2Int>();
        walkers.Add(new Walker(startPos, gridSize)); // first walker
        for (int i = 0; i < numRooms; i++)
        {
            //Move Walkers
            foreach (Walker w in walkers)
            {
                roomPosToInit.Add(w.Step());
                //add a doorway to the room before towards these rooms
            }
            //Initialize rooms
            foreach (Vector2Int v2i in roomPosToInit)
            {
                //check to see if room is already made
                if (initRooms.Contains(v2i))
                {
                    //update the init rooms doorway to connect to prev room
                }
                else
                {
                    //initialize room
                    //add doorway to previous room
                    //add room to initRooms
                }

            }

        }
    }
}

public class Walker
{
    public Vector2Int pos, prevPos, bounds;
    public int direction;

    public Walker()
    {
        pos = new Vector2Int(0, 0);
        bounds = new Vector2Int(8, 8);
        prevPos = pos;
        direction = -1;
    }

    public Walker(Vector2Int sp, Vector2Int b)
    {
        pos = sp;
        bounds = b;
        prevPos = pos;
        direction = -1;
    }

    public Walker(int x, int y, int boundX, int boundY)
    {
        pos = new Vector2Int(x, y);
        bounds = new Vector2Int(boundX, boundY);
        prevPos = pos;
        direction = -1;
    }

    public Vector2Int Step()
    {
        prevPos = pos;
        int option = Random.Range(0, 3);
        direction = option;
        switch (option)
        {
            case 0: // up
                pos.y--;
                if (pos.y < 0) // if pos is out of bounds
                {
                    pos.y++; // reset pos
                    pos = Step(); // try again
                }
                break;
            case 1: // right
                pos.x++;
                if (pos.x >= bounds.x) // if pos is out of bounds
                {
                    pos.x--; // reset pos
                    pos = Step(); // try again
                }
                break;
            case 2: // down
                pos.y++;
                if (pos.y >= bounds.y) // if pos is out of bounds
                {
                    pos.y--; // reset pos
                    pos = Step(); // try again
                }
                break;
            case 3: // left
                pos.x--;
                if (pos.x < 0) // if pos is out of bounds
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