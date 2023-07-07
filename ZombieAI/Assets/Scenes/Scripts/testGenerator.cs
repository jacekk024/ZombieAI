using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public class testGenerator : MonoBehaviour
{
    public GameObject[] rooms;
    public Vector2 gridSize;
    public int numberOfRooms;
    public GameObject player;
    public GameObject door;
    public  List<openDoors> activeRooms;
    private Vector2 gridWorldSize;
    private openDoors[,] grid;
    private List<NavMeshSurface> surfaces = new List<NavMeshSurface>();

    void Start()
    {
        gridWorldSize = new Vector2(gridSize.x * 21, gridSize.y * 21);
        GenerateMap();
        GenerateNavMesh();
    }

    void GenerateMap()
    {
        int counterRooms = 0;

        grid = new openDoors[(int)gridSize.x, (int)gridSize.y];
        activeRooms = new List<openDoors>();
        //Debug.Log("Zrobił to");
        //GameObject.Find("WaveController").GetComponent<WaveSpawner>().floor = GameObject.FindGameObjectsWithTag("ZombieRespawnZone");

        CreateStartingRoom();

        while (activeRooms.Count < numberOfRooms && ((int)gridSize.x * (int)gridSize.y) > activeRooms.Count)
        {
            if (!AddRoom()) counterRooms++;
            else counterRooms = 0;

            if (counterRooms > 100) break;
        }

        createEndDoors();
        spawnPlayer();
    }

    private void spawnPlayer()
    {
        var avaRooms = activeRooms.Where(x => x.openCenter == false).ToArray();  //x.GetComponent<openDoors>().openCenter == false).ToArray();
        var room = avaRooms[Random.Range(0, avaRooms.Count())];
        int[] yn = { 1, -1 };
        player.transform.position = new Vector3(room.transform.localPosition.x // + 22.0f // * Random.Range(0, yn.Length)
                          , 1.5f
                          , room.transform.localPosition.z // + 22.0f // * Random.Range(0, yn.Length)  
                          );
    }

    void GenerateNavMesh()
    {
        GameObject[] floors = GameObject.FindGameObjectsWithTag("ZombieRespawnZone"); // Ground

        NavMeshSurface surface = floors.First().AddComponent<NavMeshSurface>();
        surface.layerMask = LayerMask.GetMask("Ground"); // Ground
        surfaces.Add(surface);

        surface.BuildNavMesh();
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
        newRoomObj.name = (newPos.x + " - " + newPos.y).ToString();
        newRoom.gridPos = newPos;

        bool[] newAvaibleDoors = updateDoors(newRoom, temp);
        newRoom.north = newAvaibleDoors[0];
        newRoom.east = newAvaibleDoors[1];
        newRoom.south = newAvaibleDoors[2];
        newRoom.west = newAvaibleDoors[3];

        if (direction == 0) newRoom.south = false;
        else if (direction == 1) newRoom.west = false;
        else if (direction == 2) newRoom.north = false;
        else if (direction == 3) newRoom.east = false;

        grid[(int)newPos.x, (int)newPos.y] = newRoom;
        activeRooms.Add(newRoom);

        // Debug.Log(newRoomObj.name + " N: " + newRoom.north + " E: " + newRoom.east + " S: " + newRoom.south + " W: " + newRoom.west + " DIRECTION: " + direction);

        return true;
    }

    private void createEndDoors()
    {
        //bool checkRoom;

        foreach (var room in activeRooms) {
            /*if (room.gridPos.y + 1 < gridSize.y)
            {
                if (grid[(int)room.gridPos.x, (int)room.gridPos.y + 1] == null)
                    checkRoom = true;
                else checkRoom = false;
            } else checkRoom = false;*/

            if (room.north) { //  && checkRoom == false
                var doorToAdd = Instantiate(door, transform.position, transform.rotation);
                doorToAdd.transform.SetParent(room.transform); 
                doorToAdd.transform.position = new Vector3(room.transform.position.x - 3.0f, 1.0f, room.transform.position.z + 21.0f); 
                room.north = false; 

                switch (room.transform.rotation.eulerAngles.y)
                {
                    case 270.0f:
                        doorToAdd.transform.localRotation = Quaternion.Euler(0, 180, 0);
                        break;
                    case 180.0f:
                        doorToAdd.transform.localRotation = Quaternion.Euler(0, 270, 0);
                        break;
                    case 90.0f:
                        doorToAdd.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        break;
                    case 0.0f:
                        doorToAdd.transform.localRotation = Quaternion.Euler(0, 90, 0);
                        break;
                    default:
                        break;
                }
            }

            /*if (room.gridPos.x + 1 < gridSize.x)
            {
                if (grid[(int)room.gridPos.x + 1, (int)room.gridPos.y] == null)
                    checkRoom = true;
                else checkRoom = false;
            }
            else checkRoom = false;*/

            if (room.east) // && checkRoom == false
            {
                var doorToAdd = Instantiate(door, transform.position, transform.rotation);
                doorToAdd.transform.SetParent(room.transform);
                doorToAdd.transform.position = new Vector3(room.transform.position.x + 21.0f, 1.0f, room.transform.position.z - 3.0f);
                room.east = false;

                switch (room.transform.rotation.eulerAngles.y)
                {
                    case 270.0f:
                        doorToAdd.transform.localRotation = Quaternion.Euler(0, 90, 0);
                        break;
                    case 180.0f:
                        doorToAdd.transform.localRotation = Quaternion.Euler(0, 180, 0);
                        break;
                    case 90.0f:
                        doorToAdd.transform.localRotation = Quaternion.Euler(0, 270, 0);
                        break;
                    case 0.0f:
                        doorToAdd.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        break;
                    default:
                        break;
                }
            }
            
            /*if(room.gridPos.y - 1 > 0)
            {
                if (grid[(int)room.gridPos.x, (int)room.gridPos.y - 1] == null)
                    checkRoom = true;
                else checkRoom = false;
            } else checkRoom = false;*/

            if (room.south) // && checkRoom == false
            {
                var doorToAdd = Instantiate(door, transform.position, transform.rotation);
                doorToAdd.transform.SetParent(room.transform);
                doorToAdd.transform.position = new Vector3(room.transform.position.x - 3.0f, 1.0f, room.transform.position.z - 21.0f);
                room.south = false;

                switch (room.transform.rotation.eulerAngles.y)
                {
                    case 270.0f:
                        doorToAdd.transform.localRotation = Quaternion.Euler(0, 180, 0);
                        break;
                    case 180.0f:
                        doorToAdd.transform.localRotation = Quaternion.Euler(0, 270, 0);
                        break;
                    case 90.0f:
                        doorToAdd.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        break;
                    case 0.0f:
                        doorToAdd.transform.localRotation = Quaternion.Euler(0, 90, 0);
                        break;
                    default:
                        break;
                }
            }

            /*if (room.gridPos.x - 1 > 0)
            {
                if (grid[(int)room.gridPos.x - 1, (int)room.gridPos.y] == null)
                    checkRoom = true;
                else checkRoom = false;
            } else checkRoom = false;*/

            if (room.west) //  && checkRoom == false
            {
                var doorToAdd = Instantiate(door, transform.position, transform.rotation);
                doorToAdd.transform.SetParent(room.transform);
                doorToAdd.transform.position = new Vector3(room.transform.position.x - 21.0f, 1.0f, room.transform.position.z - 3.0f);
                room.west = false;

                switch (room.transform.rotation.eulerAngles.y)
                {
                    case 270.0f:
                        doorToAdd.transform.localRotation = Quaternion.Euler(0, 90, 0);
                        break;
                    case 180.0f:
                        doorToAdd.transform.localRotation = Quaternion.Euler(0, 180, 0);
                        break;
                    case 90.0f:
                        doorToAdd.transform.localRotation = Quaternion.Euler(0, 270, 0);
                        break;
                    case 0.0f:
                        doorToAdd.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        break;
                    default:
                        break;
                }
            }
        }
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