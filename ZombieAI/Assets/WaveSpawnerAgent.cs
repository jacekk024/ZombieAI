using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class WaveSpawnerAgent : Agent
{
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject zombiePrefab;
    [SerializeField] public WaveSpawner waveSpawner;

    private Vector3 spawnArea;

    public float minimumSafeDistance;
    public float maximumRewardDistance;

    private void Start()
    {
        waveSpawner = GetComponent<WaveSpawner>();
    }

    public override void Initialize()
    {
        spawnArea = this.transform.position;
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            AddReward(-1.0f);
            EndEpisode();
            Destroy(other.gameObject);
        }
    }*/

    public override void OnEpisodeBegin()
    {
        // TODO: Co resetować co epizod?
        // TODO: Pomysły: Resetowanie pozycji gracza? Jego kierunek poruszania się?
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(player.transform.position);
    }

    public override void OnActionReceived(ActionBuffers action)
    {
        float x = Mathf.Clamp(action.ContinuousActions[0], -1f, 1f);
        float z = Mathf.Clamp(action.ContinuousActions[1], -1f, 1f);

        Vector3 spawnPosition = new Vector3(spawnArea.x + x, spawnArea.y, spawnArea.z + z);
        GameObject zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);

        float distanceToPlayer = Vector3.Distance(player.transform.position, zombie.transform.position);

        if (distanceToPlayer < minimumSafeDistance)
        {
            AddReward(-1.0f);
        }
        else if (distanceToPlayer < maximumRewardDistance)
        {
            AddReward(1.0f);
        }
    }

    public void PlayerDied()
    {
        AddReward(1.0f);
        EndEpisode();
    }

    public void ZombieDied()
    {
        AddReward(-0.2f);
    }
}
