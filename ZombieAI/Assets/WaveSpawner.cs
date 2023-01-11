using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public GameObject generator;
    public GenerateGrid generateGrid;

    [SerializeField] private uint NoWave = 0;
    [SerializeField][Range(1,120)] private uint TimeOfWave;
    [SerializeField][Range(1, 30)] private uint TimeBetweenWaves;
    [SerializeField] private AvaStates State = AvaStates.Spawning;

    public ZombieType[] zombies;
    public double waveCountdown;

    [System.Serializable]
    public class ZombieType {
        public string name;
        public Transform transform;
        public uint count;
        public double rate;
    }

    private enum AvaStates
    {
        Spawning,   // Generuj zombie
        Counting,   // Oczekiwanie na zabicie zombie
        Waiting     // Przerwa, wolny czas pomiędzy falami
    };

    void Awake()
    {
        this.NoWave = 1;
        this.waveCountdown = this.TimeBetweenWaves;
        this.generateGrid = generator.GetComponent<GenerateGrid>();
    }

    void Update()
    {
        if(this.waveCountdown <= 0)
        {
            if(this.State != AvaStates.Spawning )
            {
                // Generuj zombie
                StartCoroutine(SpawnWave()); // Argument jako losowy typ zombie?
            }
        } else
        {
            this.waveCountdown -= Time.deltaTime;
        }
    }

    IEnumerator SpawnWave()
    {
        this.State = AvaStates.Spawning;



        // this.State = AvaStates.Waiting;

        yield break;
    }

    private void NextWave() => this.NoWave++;
}
