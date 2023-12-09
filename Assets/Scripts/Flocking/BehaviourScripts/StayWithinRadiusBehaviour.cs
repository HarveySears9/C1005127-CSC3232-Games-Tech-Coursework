using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flocking/Behaviour/StayWithinRadius")]
public class StayWithinRadiusBehaviour : FlockingBehavior
{
    public float radius = 1f;

    public override Vector2 CalculateMove(FlockingAgent agent, List<Transform> context, Flock flock)
    {
        // Keeps the agents centered around the flock game objects positions
        Vector2 centerOffset = (Vector2)flock.transform.position - (Vector2)agent.transform.position;
        float t = centerOffset.magnitude / radius;

        if(t < 0.9)
        {
            return Vector2.zero;
        }

        return centerOffset * t * t;
    }
}
