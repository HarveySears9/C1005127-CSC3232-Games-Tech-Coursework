using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// can't instaniate an abstract class
public abstract class FlockingBehavior : ScriptableObject
{
    public abstract Vector2 CalculateMove(FlockingAgent agent, List<Transform> context, Flock flock);
}
