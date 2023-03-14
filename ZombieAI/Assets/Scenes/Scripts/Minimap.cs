using UnityEngine;

public class Minimap : MonoBehaviour
{
    [SerializeField] private Transform player;

    void Update()
    {
        transform.position = player.position + Vector3.up * 5f;
    }
}