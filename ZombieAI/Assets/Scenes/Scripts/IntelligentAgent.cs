using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class IntelligentAgent : Agent
{
    [SerializeField] private Transform treasure;
    [SerializeField] private Transform spikes;

    //CharacterState characterState;

    private void Start()
    {
        //characterState = GetComponent<CharacterState>();
    }

    public override void OnEpisodeBegin()
    {
        float[] possiblePositionX = new float[] { -3.5f, 3.5f };
        int randomIndex = Random.Range(0, possiblePositionX.Length);
        float positionX = possiblePositionX[randomIndex];

        treasure.transform.position = new Vector3(positionX, -.5f, 0);
        spikes.transform.position = (randomIndex == 0) ? new Vector3(possiblePositionX[1], -.5f, 0) : new Vector3(possiblePositionX[0], -.5f, 0);

        transform.position = new Vector3(2, 0, 0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {

    }

    public override void OnActionReceived(ActionBuffers actions)
    {

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }
}