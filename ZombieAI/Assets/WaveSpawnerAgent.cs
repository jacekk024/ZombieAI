using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using static WaveSpawner;

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

    public override void OnEpisodeBegin()
    {
        // TODO: Co resetować co epizod?
        // TODO: Pomysły: Resetowanie pozycji gracza? Jego kierunek poruszania się? Jego hp ma reset. Jeszcze ilość strzał

        player.gameObject.transform.position = new Vector3(-16.0f, 1.5f, 11.0f); // Reset position
        player.gameObject.transform.rotation = new Quaternion(0, 0, 0, 1); // Reset rotation
        // player.GetComponent<PlayerMove>().currentHealth = player.GetComponent<PlayerMove>().maxHealth; // TODO: Fix it

        // StartCoroutine(waveSpawner.SpawnWave(waveSpawner.zombies[waveSpawner.NoWave]));

        if (waveSpawner.State != AvaStates.Spawning)
        {
            StartCoroutine(waveSpawner.SpawnWave(waveSpawner.zombies[waveSpawner.NoWave]));
        }

        // Dodanie resetowania strzał? Nie potrzebne, bo nie zdąży wykorzystać
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
        string colliderTag = waveSpawner.ColliderBetweenObjAndPlayer(spawnPosition);

        ValidationPosition(spawnPosition, colliderTag);
        // Instantiate(zombiePrefab, spawnPosition, Quaternion.identity); // ???
    }

    private void ValidationPosition(Vector3 spawnPosition, string colliderTag)
    {
        float distanceToPlayer = Vector3.Distance(player.transform.position, spawnPosition);

        if (distanceToPlayer < minimumSafeDistance)
        {
            GiveReward("UnderSafeDistance");
        } else if (distanceToPlayer > maximumRewardDistance)
        {
            GiveReward("AboveMaximumDistance");
        }

        if(colliderTag == "Player")
        {
            GiveReward("PlayerSeeZombie");
        } else
        {
            GiveReward("PlayerCantSeeZombie");
        }
    }

    private void GiveReward(string whatFor)
    {
        switch(whatFor)
        {
            case "UnderSafeDistance":
                AddReward(-0.6f);
                break;
            case "AboveMaximumDistance":
                AddReward(-0.2f);
                break;
            case "PlayerDied":
                AddReward(10.0f);
                break;
            case "ZombieDied":
                AddReward(-0.5f);
                break;
            case "PlayerSeeZombie":
                AddReward(-0.5f);
                break;
            case "PlayerCantSeeZombie":
                AddReward(-0.5f);
                break;
        }
    }

    public void PlayerDied()
    {
        GiveReward("PlayerDied");
        EndEpisode();
    }

    public void ZombieDied()
    {
        GiveReward("ZombieDied");
    }
}
