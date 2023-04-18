using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class ZombieAgent : Agent
{
    public Transform player;
    public float moveSpeed = 5.0f;
    public PlayerInputActuator playerInputActuator;
    public List<IActuator> Actuators;

    public float timeToDetectEvade = 1.0f;
    private float evadeTimer;
    private float lastDistanceToProjectile;
    public GameObject projectile;

    public override void Initialize()
    {
        playerInputActuator = new PlayerInputActuator(this);
        Actuators = new List<IActuator> { playerInputActuator };
    }

    private void Update()
    {
        float currentDistanceToProjectile = Vector3.Distance(transform.position, projectile.transform.position);

        if (currentDistanceToProjectile > lastDistanceToProjectile)
        {
            evadeTimer += Time.deltaTime;
        }
        else
        {
            evadeTimer = 0;
        }

        lastDistanceToProjectile = currentDistanceToProjectile;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            AddReward(-1.0f);
            EndEpisode();
            Destroy(other.gameObject);
        }
    }


    public override void OnEpisodeBegin()
    {
        // Reset zombie/gracza
        GameObject.Find("WaveController").GetComponent<WaveSpawner>().State = WaveSpawner.AvaStates.Spawning;

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(player.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);

        Vector3 moveDirection = new Vector3(actions.ContinuousActions[0], 0, actions.ContinuousActions[1]);
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        if (evadeTimer >= timeToDetectEvade)
        {
            AddReward(1.0f);
            evadeTimer = 0;
        }
    }
}
