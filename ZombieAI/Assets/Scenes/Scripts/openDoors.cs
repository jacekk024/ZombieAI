using UnityEngine;

public class openDoors : MonoBehaviour
{
    [SerializeField] public Vector2 gridPos;
    [SerializeField] public bool north = false; // 0
    [SerializeField] public bool east = false;  // 1
    [SerializeField] public bool south = false; // 2
    [SerializeField] public bool west = false;  // 3
}
