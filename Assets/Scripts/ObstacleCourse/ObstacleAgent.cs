using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

[RequireComponent(typeof(RayPerceptionSensorComponent3D), typeof(DecisionRequester))]
public class ObstacleAgent : Agent
{
    Rigidbody rb;
    [SerializeField] float speed = 5f;
    [SerializeField] float maxTurnSpeed = 5f;

    // Can be used as the Start() function
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Runs every time the training environment is reset (when a new episode begins) 
    public override void OnEpisodeBegin()
    {

    }

    /// The following 3 functions can be seen as running like the Update() loop, except they run as frequently as you collect observations: 
    /// first, CollectObservations() gets the input vector for the DQN, 
    /// then Heuristic() can override the actual output action vector (basically for testing only)
    /// finally, OnActionReceived() takes the output action vector and moves the Agent

    // Collect input observations (corresponding with the BrainParameters property above)
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position.x);
        sensor.AddObservation(transform.position.z);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var contOut = actionsOut.ContinuousActions;

        contOut[0] = Input.GetAxisRaw("Horizontal");
        contOut[1] = Input.GetAxisRaw("Vertical");
        contOut[2] = 0; // Placeholder
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Our agent will be able to freely control its non-vertical movement.
        rb.velocity = new Vector3(Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f) * speed, rb.velocity.y, Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f) * speed);
        rb.angularVelocity = new Vector3(rb.angularVelocity.x, Mathf.Clamp(actions.ContinuousActions[2], -1f, 1f) * maxTurnSpeed, rb.angularVelocity.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("boundary"))
        {
            AddReward(-2f);
            //EndEpisode();
        }
        if (collision.gameObject.CompareTag("Finish"))
        {
            AddReward(50f);
            EndEpisode();
        }
    }
}
