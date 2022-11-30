using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform targetTransform;

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();

        transform.position = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //base.CollectObservations(sensor);
        sensor.AddObservation(transform.position);
        sensor.AddObservation(targetTransform.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //base.OnActionReceived(actions);
        //Debug.Log(actions.DiscreteActions[0]);
        //Debug.Log(actions.ContinuousActions[0]);

        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 5.0f;
        transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //base.Heuristic(actionsOut);

        ActionSegment<float> contiuousActions = actionsOut.ContinuousActions;

        contiuousActions[0] = Input.GetAxis("Horizontal");
        contiuousActions[1] = Input.GetAxis("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Rewardcollider>(out Rewardcollider rewardcollider))
        {
            Debug.Log("Hit reward");
            SetReward(+1f);
            EndEpisode();
        }

        if (other.TryGetComponent<Wallcollider>(out Wallcollider wallcollider))
        {
            Debug.Log("Hit wall");
            SetReward(-1f);
            EndEpisode();
        }
        // AddReward()
    }
}
