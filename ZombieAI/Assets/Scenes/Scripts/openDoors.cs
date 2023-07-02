using UnityEngine;

public class openDoors : MonoBehaviour
{
    [SerializeField] public Vector2 gridPos;
    [SerializeField] public bool north = false; // 0
    [SerializeField] public bool east = false;  // 1
    [SerializeField] public bool south = false; // 2
    [SerializeField] public bool west = false;  // 3
    [SerializeField] public bool door1 = false; // 0
    [SerializeField] public bool door2 = false; // 1
    [SerializeField] public bool door3 = false; // 2
    [SerializeField] public bool door4 = false; // 3
    [SerializeField] public bool openCenter = false;
}
