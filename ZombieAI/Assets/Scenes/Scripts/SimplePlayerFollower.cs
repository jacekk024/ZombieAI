using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimplePlayerFollower : MonoBehaviour
{
    [Header("Functional Options")]
    [SerializeField] private bool IsActive = true;

    [Header("References")]
    [SerializeField] private Transform target;
    
    NavMeshAgent nav;

    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(IsActive)
        {
            nav.SetDestination(target.position);
        }
    }
}
