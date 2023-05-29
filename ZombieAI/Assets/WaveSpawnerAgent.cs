using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class WaveSpawnerAgent : Agent
{
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject zombiePrefab;
    [SerializeField] public WaveSpawner waveSpawner;
    [SerializeField] public int episodeCounter = 0;
    [SerializeField] public int limitZombieToSpawn = 5;
    [SerializeField] public int zombieCounter = 0;
    [SerializeField] public bool isLearning = false;
    [SerializeField] public float minimumSafeDistance = 5.0f;
    [SerializeField] public float maximumRewardDistance = 20.0f;

    private void Start()
    {
        waveSpawner = GetComponent<WaveSpawner>();
    }

    public override void OnEpisodeBegin()
    {
        if (isLearning)
        {
            episodeCounter++;
            Debug.Log("RESET EPISODE!! Episode no: " + episodeCounter);

            player.transform.localPosition = new Vector3(-16.0f, 1.5f, 11.0f);
            player.transform.localRotation = new Quaternion(0, 0, 0, 1);
            player.GetComponent<PlayerMove>().PlayerGunComponent.Reload();
            player.GetComponent<PlayerMove>().currentHealth = player.GetComponent<PlayerMove>().maxHealth;

            // Aktualny stan zdrowia jest ustawiony na 100, trzeba zaaktualizować UI, że ma full hp
            // Debug.Log("Current Health: " + player.GetComponent<PlayerMove>().currentHealth + ", Max Health: " + player.GetComponent<PlayerMove>().maxHealth);

            DestroyAllZombies();
            zombieCounter = 0;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(player.transform.localPosition);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Random.Range(-50.0f, 50.0f);
        continuousActions[1] = Random.Range(-50.0f, 50.0f);
    }

    public override void OnActionReceived(ActionBuffers action)
    {
        if(zombieCounter < limitZombieToSpawn)
        {
            zombieCounter++;

            float x = Mathf.Clamp(action.ContinuousActions[0], -50.0f, 50.0f);
            float z = Mathf.Clamp(action.ContinuousActions[1], -50.0f, 50.0f);
            // Debug.Log("ContinuousActions X: " + x + ", ContinuousActions Z: " + z); // Zostawiam do debugowania

            Vector3 spawnPosition = new Vector3(player.gameObject.transform.localPosition.x + x
                , player.transform.localPosition.y
                , player.transform.localPosition.z + z);

            //string colliderTag = waveSpawner.ColliderBetweenObjAndPlayer(spawnPosition); // Tutaj wywala, trzeba ochronić

            // Debug.Log("X: " + spawnPosition.x + ", Z: " + spawnPosition.z); // Zostawiam do debugowania
            GameObject.Find("Heatmap").GetComponent<RaycastGradient>().UpdateHeatTexture(spawnPosition.x, spawnPosition.z);
            GameObject go = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
            player.GetComponent<PlayerMove>().AddTarget(go.transform);
            //ValidationPosition(spawnPosition, colliderTag); // Tutaj wywala, trzeba ochronić, wywoływane przez colliderTag
        }
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
                AddReward(-0.5f);
                break;
            case "AboveMaximumDistance":
                AddReward(-0.15f);
                break;
            case "PlayerDied":
                AddReward(1.0f);
                break;
            case "ZombieDied":
                AddReward(-0.5f);
                break;
            case "PlayerSeeZombie":
                AddReward(-0.5f);
                break;
            case "PlayerCantSeeZombie":
                AddReward(0.1f);
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

    void DestroyAllZombies()
    {
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Enemy");

        for (var i = 0; i < zombies.Length; i++)
        {
            zombies[i].GetComponent<ZombieController>().Die();
        }
    }
}
