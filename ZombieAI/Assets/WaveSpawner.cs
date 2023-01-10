using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private uint NoWave = 0;
    [SerializeField][Range(1,120)] private uint TimeOfWave;
    [SerializeField][Range(1, 30)] private uint TimeBetweenWaves;
    [SerializeField] private AvaStates State = AvaStates.Spawning;

    private enum AvaStates
    {
        Spawning,   // Generuj zombie
        Counting,   // Oczekiwanie na zabicie zombie
        Waiting     // Przerwa, wolny czas pomiędzy falami
    };

    void Start()
    {
        this.NoWave = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void NextWave() => this.NoWave++;
}
