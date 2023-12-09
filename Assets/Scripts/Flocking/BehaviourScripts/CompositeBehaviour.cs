using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flocking/Behaviour/Composite")]
public class CompositeBehaviour : FlockingBehavior
{
    public FlockingBehavior[] behaviours;
    public float[] weights;

    public override Vector2 CalculateMove(FlockingAgent agent, List<Transform> context, Flock flock)
    {
        // Handle data mismatch
        if(weights.Length != behaviours.Length)
        {
            Debug.LogError("need same length in both arrays in " + name, this);
            return Vector2.zero;
        }

        //Set up move
        Vector2 move = Vector2.zero;

        // Iterate through behaviours
        for (int i = 0; i < behaviours.Length; i++)
        {
            Vector2 partialMove = behaviours[i].CalculateMove(agent, context, flock) * weights[i];

            if(partialMove != Vector2.zero)
            {
                if(partialMove.sqrMagnitude > weights[i] * weights[i])
                {
                    partialMove = partialMove.normalized;
                    partialMove *= weights[i];
                }

                move += partialMove;
            }
        }

        return move;
    }
}
