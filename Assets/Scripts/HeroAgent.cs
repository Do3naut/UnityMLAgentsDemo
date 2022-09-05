using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

[RequireComponent(typeof(RayPerceptionSensorComponent2D), typeof(DecisionRequester))]
public class HeroAgent : Agent
{
    [SerializeField] EnvManager env;
    [SerializeField] SpriteRenderer bg;

    private void Awake()
    {
        GetComponent<BehaviorParameters>().BrainParameters.VectorObservationSize = 2;  // For the x y of the player
        GetComponent<BehaviorParameters>().BrainParameters.ActionSpec = new ActionSpec(0, new[] { 3, 3 });  // Movement in x y directions
    }

    // Can be used as the Start() function
    public override void Initialize()
    {
        
    }

    // Runs every time the training environment is reset (when a new episode begins) 
    public override void OnEpisodeBegin()
    {
        env.SpawnEnemies();
    }

    /// The following 3 functions can be seen as running in the Update() loop: 
    /// first, CollectObservations() gets the input vector for the DQN, 
    /// then Heuristic() can override the actual output action vector (basically for testing only)
    /// finally, OnActionReceived() takes the output action vector and moves the Agent

    // Collect input observations (corresponding with the BrainParameters property above)
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position.x);
        sensor.AddObservation(transform.position.y);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discOut = actionsOut.DiscreteActions;

        discOut[0] = (int)Input.GetAxisRaw("Horizontal");
        discOut[1] = (int)Input.GetAxisRaw("Vertical");
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        transform.position += new Vector3(actions.DiscreteActions[0] - 1, actions.DiscreteActions[1] - 1, 0) * Time.deltaTime;
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("boundary"))
        {
            AddReward(-50f);
            EndEpisode();
        }
    }

    public void Win()
    {
        AddReward(50f);
        bg.color = Color.green;
        EndEpisode();
    }

    public void EnemyDestroyed()
    {
        AddReward(10f);
    }
}
