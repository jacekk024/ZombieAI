﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class testGenerator : MonoBehaviour
{
    public GameObject[] rooms;
    public Vector2 gridSize;
    public int numberOfRooms;

    private Vector2 gridWorldSize;
    private openDoors[,] grid;
    private List<openDoors> activeRooms;

    void Start()
    {
        gridWorldSize = new Vector2(gridSize.x * 21, gridSize.y * 21);
        GenerateMap();
    }

    void GenerateMap()
    {
        int counterRooms = 0;

        grid = new openDoors[(int)gridSize.x, (int)gridSize.y];
        activeRooms = new List<openDoors>();
        CreateStartingRoom();

        while (activeRooms.Count < numberOfRooms && ((int)gridSize.x * (int)gridSize.y) > activeRooms.Count)
        {
            if (!AddRoom()) counterRooms++;
            else counterRooms = 0;

            if (counterRooms > 100) break;
        }
    }

    public void RegenerateMap()
    {
        foreach (openDoors room in activeRooms)
        {
            Destroy(room.gameObject);
        }
        GenerateMap();
    }

    void CreateStartingRoom()
    {
        Vector2 startPos = new Vector2((int)gridSize.x / 2, (int)gridSize.y / 2);
        GameObject startingRoomObj = Instantiate(rooms[Random.Range(0, rooms.Length)], CoordToPosition(startPos), Quaternion.identity);
        openDoors startingRoom = startingRoomObj.GetComponent<openDoors>();

        startingRoom.name = "StartRoom";
        startingRoom.gridPos = startPos;

        grid[(int)startPos.x, (int)startPos.y] = startingRoom;
        activeRooms.Add(startingRoom);
    }

    Vector3 CoordToPosition(Vector2 coord) 
    {
        return new Vector3((-gridWorldSize.x) + (coord.x * 42), 0, (-gridWorldSize.y) + (coord.y * 42));
    }

    bool AddRoom()
    {
        openDoors currentRoom = activeRooms[Random.Range(0, activeRooms.Count)];
        List<int> possibleDirections = new List<int>();
        Vector2 newPos = currentRoom.gridPos;
        Quaternion newRoomRotation = Quaternion.identity;

        if (newPos.y < gridSize.y - 1 && grid[(int)newPos.x, (int)(newPos.y + 1)] == null && currentRoom.north) possibleDirections.Add(0);
        if (newPos.x < gridSize.x - 1 && grid[(int)(newPos.x + 1), (int)newPos.y] == null && currentRoom.east)  possibleDirections.Add(1);
        if (newPos.y > 0 && grid[(int)newPos.x, (int)(newPos.y - 1)] == null && currentRoom.south)  possibleDirections.Add(2);
        if (newPos.x > 0 && grid[(int)(newPos.x - 1), (int)newPos.y] == null && currentRoom.west)   possibleDirections.Add(3);
        if (possibleDirections.Count == 0) return false;

        int direction = possibleDirections[Random.Range(0, possibleDirections.Count)];

        if (direction == 0) // North
        {
            newPos += Vector2.up;
            currentRoom.north = false;
        }
        else if (direction == 1) // East
        {
            newPos += Vector2.right;
            currentRoom.east = false;
        }
        else if (direction == 2) // South
        {
            newPos += Vector2.down;
            currentRoom.south = false;
        }
        else if (direction == 3) // West
        {
            newPos += Vector2.left;
            currentRoom.west = false;
        }

        GameObject newRoomObj = Instantiate(rooms[Random.Range(0, rooms.Length)], CoordToPosition(newPos), Quaternion.identity);
        openDoors newRoom = newRoomObj.GetComponent<openDoors>();

        List<int> possibleExits = new List<int>();

        if (newRoom.north) possibleExits.Add(0);
        if (newRoom.east)  possibleExits.Add(1);
        if (newRoom.south) possibleExits.Add(2);
        if (newRoom.west)  possibleExits.Add(3);

        int exit = possibleExits[Random.Range(0, possibleExits.Count)];

        int temp = 0;
        int obrot = direction - exit;

        if (obrot < 0) obrot += 4;

        switch (obrot)
        {
            case 0:
                newRoomRotation = Quaternion.Euler(0, 180, 0);
                temp = 2;
                break;
            case 1:
                newRoomRotation = Quaternion.Euler(0, 270, 0);
                temp = 3;
                break;
            case 2:
                newRoomRotation = Quaternion.Euler(0, 0, 0);
                temp = 0;
                break; 
            case 3:
                newRoomRotation = Quaternion.Euler(0, 90, 0);
                temp = 1;
                break;
            default:
                break;
        }

        newRoomObj.transform.rotation = newRoomRotation;
        newRoom.gridPos = newPos;

        bool[] newAvaibleDoors = updateDoors(newRoom, temp);
        newRoom.north = newAvaibleDoors[0];
        newRoom.east = newAvaibleDoors[1];
        newRoom.south = newAvaibleDoors[2];
        newRoom.west = newAvaibleDoors[3];

        if (newRoom.gridPos.y + 1 >= gridSize.y) newRoom.north = false;
        else if(grid[(int)newRoom.gridPos.x, (int)newRoom.gridPos.y + 1] != null)
        {
            newRoom.north = false;
            activeRooms.Where(x => x.gridPos.x == newPos.x && x.gridPos.y == (newPos.y + 1)).First().south = false;
        }

        if (newRoom.gridPos.x + 1 >= gridSize.x) newRoom.east = false;
        else if (grid[(int)newRoom.gridPos.x + 1, (int)newRoom.gridPos.y] != null)
        {
            newRoom.east = false;
            activeRooms.Where(x => x.gridPos.x == (newPos.x + 1) && x.gridPos.y == newPos.y).First().west = false;
        }

        if (newRoom.gridPos.y - 1 < 0) newRoom.south = false;
        else if (grid[(int)newRoom.gridPos.x, (int)newRoom.gridPos.y - 1] != null)
        {
            newRoom.south = false;
            activeRooms.Where(x => x.gridPos.x == newPos.x && x.gridPos.y == (newPos.y - 1)).First().north = false;
        }

        if (newRoom.gridPos.x - 1 < 0)
            newRoom.west = false;
        else if (grid[(int)newRoom.gridPos.x - 1, (int)newRoom.gridPos.y] != null)
        {
            newRoom.west = false;
            activeRooms.Where(x => x.gridPos.x == (newPos.x - 1) && x.gridPos.y == newPos.y).First().east = false;
        }

        grid[(int)newPos.x, (int)newPos.y] = newRoom;
        activeRooms.Add(newRoom);

        return true;
    }

    private bool[] updateDoors(openDoors room, int obrot)
    {
        bool[] exit = new bool[4];

        if (room.north) exit[obrot % 4] = true; // North
        if (room.east)  exit[(1 + obrot) % 4] = true; // East
        if (room.south) exit[(2 + obrot) % 4] = true; // South
        if (room.west)  exit[(3 + obrot) % 4] = true; // West

        return exit;
    }
}