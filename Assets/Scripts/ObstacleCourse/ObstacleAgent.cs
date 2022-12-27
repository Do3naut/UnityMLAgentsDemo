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

    [SerializeField] private Transform targetTransform;

    [SerializeField] private Material winMaterial, loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    private RayPerceptionSensorComponent3D RaySensor;

    private Vector3 startingPos;

    private float startingDistance;
    private Vector3 lastPos;

    // Can be used as the Start() function
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        startingPos = new Vector3(9, 0.5f, -9);
        RaySensor = GetComponent<RayPerceptionSensorComponent3D>();
        startingDistance = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
        lastPos = transform.localPosition;
    }

    // Runs every time the training environment is reset (when a new episode begins) 
    public override void OnEpisodeBegin()
    {
        transform.localPosition = startingPos;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    /// The following 3 functions can be seen as running like the Update() loop, except they run as frequently as you collect observations: 
    /// first, CollectObservations() gets the input vector for the DQN, 
    /// then Heuristic() can override the actual output action vector (basically for testing only)
    /// finally, OnActionReceived() takes the output action vector and moves the Agent

    // Collect input observations (corresponding with the BrainParameters property above)
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition - transform.localPosition);
        sensor.AddObservation(transform.rotation.eulerAngles);
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
        float prevDistance = Vector3.Distance(lastPos, targetTransform.localPosition);
        float currDistance = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
        AddReward((prevDistance-currDistance)/10f);
        Debug.Log(GetCumulativeReward());
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        float rotation = actions.ContinuousActions[2];
        // Our agent will be able to freely control its non-vertical movement.
        
        rb.velocity = transform.right * Mathf.Clamp(moveX, -1f, 1f) * speed + transform.forward * Mathf.Clamp(moveZ, -1f, 1f) * speed;

        Vector3 angleDelta = new Vector3(0, Mathf.Clamp(rotation, -1f, 1f) * maxTurnSpeed, 0);
        transform.Rotate(angleDelta);
        //rb.angularVelocity = new Vector3(rb.angularVelocity.x, Mathf.Clamp(rotation, -1f, 1f) * maxTurnSpeed, rb.angularVelocity.z);

        lastPos = transform.localPosition;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("boundary"))
        {
            AddReward(-0.2f);
        }
        if (collision.gameObject.CompareTag("Finish"))
        {
            AddReward(100f);
            EndEpisode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("boundary"))
        {
            AddReward(-50f);
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();

        }
        if (other.gameObject.CompareTag("Finish"))
        {
            AddReward(100f);
            floorMeshRenderer.material = winMaterial;
            EndEpisode();
        }
    }
}
