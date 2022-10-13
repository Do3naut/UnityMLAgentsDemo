using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

[RequireComponent(typeof(RayPerceptionSensorComponent3D), typeof(DecisionRequester))]
public class BRAgent : Agent
{
    [SerializeField] EnvManager env;
    [SerializeField] SpriteRenderer bg;

    private void Awake()
    {
    }

    // Can be used as the Start() function
    public override void Initialize()
    {

    }

    // Runs every time the training environment is reset (when a new episode begins) 
    public override void OnEpisodeBegin()
    {

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
