using Unity.MLAgents.Actuators;
using UnityEngine;

public class PlayerInputActuator : IActuator
{
    private ZombieAgent agent;

    public PlayerInputActuator(ZombieAgent agent)
    {
        this.agent = agent;
    }

    public void OnActionReceived(ActionBuffers actions)
    {
        // Pusty, gdyż nie potrzebujemy dodatkowego kodu dla akcji sterowanych przez gracza
    }

    public void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        // Pusty, gdyż nie potrzebujemy maskowania akcji w tym przypadku
    }

    public void Heuristic(in ActionBuffers actionBuffersOut)
    {
        // Ręczne sterowanie agentem dla testowania
        var continuousActionsOut = actionBuffersOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    public void ResetData()
    {
        throw new System.NotImplementedException();
    }

    public string Name => "PlayerInputActuator";
    public ActionSpec ActionSpec => ActionSpec.MakeContinuous(2);
}
