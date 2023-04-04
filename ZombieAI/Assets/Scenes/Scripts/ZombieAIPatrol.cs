using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAIPatrol : MonoBehaviour
{
    public GameObject player;
    public NavMeshAgent navMeshAgent;

    [SerializeField] LayerMask groundLayer, playerLayer;

    Vector3 destinationPoint;
    bool walkpointSet;
    [SerializeField] float range;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent= GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        if (!walkpointSet)
        {
            SearchForDest();
        } else
        {
            navMeshAgent.SetDestination(destinationPoint);
        }

        if (Vector3.Distance(transform.position, destinationPoint) < 10)
        {
            walkpointSet = false;
        }
    }

    void SearchForDest()
    {
        float X = Random.Range(-range, range);
        float Z = Random.Range(-range, range);

        destinationPoint = new Vector3(transform.position.x + X, transform.position.y, transform.position.z + Z);
        // Debug.Log("Chce iść do: X = " + (transform.position.x + X) + " Y: " + transform.position.y + " Z: " + (transform.position.z + Z));

        if(Physics.Raycast(destinationPoint, Vector3.down, groundLayer))
        {
            walkpointSet = true;
        }
    }
}
