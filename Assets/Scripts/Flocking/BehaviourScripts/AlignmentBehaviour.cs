using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flocking/Behaviour/Alignment")]
public class AlignmentBehaviour : FlockingBehavior
{
    public override Vector2 CalculateMove(FlockingAgent agent, List<Transform> context, Flock flock)
    {
        // if no neighbours, maintain current alignment
        if (context.Count == 0)
        {
            return agent.transform.up;
        }

        // Add all points together and average
        Vector2 alignmentMove = Vector2.zero;
        foreach (Transform item in context)
        {
            alignmentMove += (Vector2)item.transform.up;
        }
        alignmentMove /= context.Count;

        return alignmentMove;
    }
}
