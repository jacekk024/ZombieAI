using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimplePlayerFollower : MonoBehaviour
{
    [Header("Functional Options")]
    [SerializeField] private bool IsActive = true;

    [Header("References")]
    [SerializeField] public Transform target;
    
    NavMeshAgent nav;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            PlayerMove.OnTakeDamage(15);
    }


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
