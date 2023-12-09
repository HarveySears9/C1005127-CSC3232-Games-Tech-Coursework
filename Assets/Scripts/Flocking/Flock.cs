using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public FlockingAgent agentPrefab;
    List<FlockingAgent> agents = new List<FlockingAgent>();
    public FlockingBehavior behavior;

    [Range(1, 25)]
    public int minStartingPopulation = 5;
    public int maxStartingPopulation = 10;
    const float AgentDensity = 0.08f;

    [Range(1f, 100f)]
    public float driveFactor = 10f;
    [Range(0f, 5f)]
    public float maxSpeed = 5f;
    [Range(0.1f, 1f)]
    public float neighborRadius = 0.1f;
    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f;

    float squareMaxSpeed;
    float squareNeighbourRadius;
    float squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }

    // Start is called before the first frame update
    void Start()
    {
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighbourRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareNeighbourRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

        // used to calculate spawn points of flock around the object itself
        Transform centerTransform = this.transform;

        int startingPopulation = Random.Range(minStartingPopulation, maxStartingPopulation);

        for (int i = 0; i < startingPopulation; i++)
        {
            // Calculate a random position around centerObject
            Vector2 randomCirclePos = Random.insideUnitCircle * startingPopulation * AgentDensity;
            Vector3 spawnPosition = new Vector3(randomCirclePos.x, randomCirclePos.y, 0f) + centerTransform.position;

            FlockingAgent newAgent = Instantiate(
                agentPrefab,
                spawnPosition,
                Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)),
                transform
                );
            newAgent.name = "Agent " + i;
            agents.Add( newAgent );
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(FlockingAgent agent in agents)
        {
            List<Transform> context = GetNearbyObjects(agent);
            
            // checking context
            //agent.GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(Color.white, Color.red, context.Count / 6f);

            Vector2 move = behavior.CalculateMove(agent, context, this);
            move *= driveFactor;

            if(move.sqrMagnitude > squareMaxSpeed)
            {
                move = move.normalized * maxSpeed;
            }
            agent.Move(move);
        }
    }

    List<Transform> GetNearbyObjects(FlockingAgent agent)
    {
        List<Transform> context = new List<Transform>();
        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighborRadius);
        foreach(Collider2D c in contextColliders)
        {
            if(c != agent.AgentCollider)
            {
                context.Add(c.transform);
            }
        }
        return context;
    }
}
