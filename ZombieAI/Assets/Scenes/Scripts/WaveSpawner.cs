using System.Collections;
using Unity.MLAgents;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public GameObject zombieGameObject;

    [SerializeField] public uint NoWave = 0;
    [SerializeField][Range(1, 30)] private float TimeBetweenWaves = 5.0f;
    [SerializeField] public AvaStates State = AvaStates.Counting;
    [SerializeField] public bool isLearning = false;
    [SerializeField] public Agent agent;

    public ZombieType[] zombies;
    public GameObject[] floor;
    public float waveCountdown;
    public float searchCountdown = 1.0f;

    [System.Serializable]
    public class ZombieType {       // Zombie
        public string name;         // Name of zombie
        public Transform transform; // Zombie position
        public uint count;          // Count of zombie
        public float rate;          // Zombie rate
    }

    public enum AvaStates
    {
        Spawning,   // Spawning time to generate zombies
        Counting,   // Countdown of break from waves
        Waiting     // Waiting to kill zombies
    };

    void Awake()
    {
        NoWave = 0;
        waveCountdown = TimeBetweenWaves;
        floor = GameObject.FindGameObjectsWithTag("ZombieRespawnZone");
    }

    void Update()
    {
        if (!isLearning)
        {
            // Check if zombies still alive
            if (State == AvaStates.Waiting)
            {
                if (!ZombieIsAlive())
                {
                    WaveCompleted();
                } else
                {
                    return;
                }
            }

            if (waveCountdown <= 0)
            {
                if (State != AvaStates.Spawning)
                {
                    // Generuj zombie
                    StartCoroutine(SpawnWave(zombies[Random.Range(0,zombies.Length)]));
                }
            } else
            {
                waveCountdown -= Time.deltaTime;
            }
        }
    }

    void WaveCompleted()
    {
        Debug.Log("End of Wave");

        State = AvaStates.Counting;
        waveCountdown = TimeBetweenWaves;

        if (NoWave + 1 > zombies.Length - 1)
        {
            NoWave = 0;
            Debug.Log("Loop of zombies classes");
        } else
        {
            NoWave++;
        }
    }

    bool ZombieIsAlive()
    {
        searchCountdown -= Time.deltaTime;

        if (searchCountdown <= 0f)
        {
            searchCountdown = 1f;
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        }

        return true;
    }

    public IEnumerator SpawnWave(ZombieType _zombie)
    {
        Debug.Log("Spawning Wave: " + _zombie.name);
        State = AvaStates.Spawning;

        for (int i = 0; i <= _zombie.count; i++)
        {
            SpawnEnemy(_zombie.transform);
            // yield return new WaitForSeconds(1f / _zombie.rate); // Raczej nie potrzebne spowolnienie generowania zombie
        }

        State = AvaStates.Waiting;

        yield break;
    }

    private void SpawnEnemy(Transform _zombie)
    {
        Vector3 spawnPosition;
        Vector3 positionToCheck;

        float radius = 1f;
        bool spawned = false;

        while (!spawned)
        {
            spawnPosition = positionToCheck = floor[Random.Range(0, floor.Length - 1)].transform.position;

            positionToCheck.y += 1f;

            if(Physics.CheckSphere(positionToCheck, radius) && ColliderBetweenObjAndPlayer(positionToCheck) != "Player")
            {
                Debug.Log("Zombie spawned: " + _zombie.name);

                GameObject go = 
                    Instantiate(zombieGameObject, spawnPosition, transform.rotation);
                
                // Aktualnie skrypt wyłączony, zbędny kod
                /*go.GetComponent<SimplePlayerFollower>().target = 
                    GameObject.FindGameObjectWithTag("Player").transform;*/

                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMove>().AddTarget(go.transform);

                spawned = true;
            }
        }
    }

    internal string ColliderBetweenObjAndPlayer(Vector3 vector)
    {
        Physics.Linecast(vector, GameObject.FindGameObjectWithTag("Player").transform.position, out RaycastHit raycastHit);
        Debug.Log("Hit: " + raycastHit.transform.tag);
        return raycastHit.transform.tag;
    }
}
