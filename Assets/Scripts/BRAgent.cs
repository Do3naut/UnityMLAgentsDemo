using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

[RequireComponent(typeof(RayPerceptionSensorComponent3D), typeof(DecisionRequester))]
public class BRAgent : Agent
{
    [Header("Management")]

    [SerializeField] BRManager env;

    [Header("Gameplay")]

    Rigidbody rb;
    [SerializeField] float speed = 5f;
    [SerializeField] float maxTurnSpeed = 5f;

    [SerializeField] LayerMask targetableLayers;
    [SerializeField] float gunCooldownInSeconds = 3f;
    float cooldown = 0;

    AudioSource audio;
    [SerializeField] AudioClip shootSound;
    [SerializeField] AudioClip killSound;

    private void Awake()
    {
    }

    // Can be used as the Start() function
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
    }

    // Runs every time the training environment is reset (when a new episode begins) 
    public override void OnEpisodeBegin()
    {
        env.ResetStage();
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
        sensor.AddObservation(Physics.Raycast(transform.position, transform.forward, 200f, targetableLayers));
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
        // transform.position += new Vector3(actions.DiscreteActions[0] - 1, actions.DiscreteActions[1] - 1, 0) * Time.deltaTime;
        rb.velocity = new Vector3(Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f) * speed, rb.velocity.y, Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f) * speed);
        rb.angularVelocity = new Vector3(rb.angularVelocity.x, Mathf.Clamp(actions.ContinuousActions[2], -1f, 1f) * maxTurnSpeed, rb.angularVelocity.z);
        if (actions.DiscreteActions[0] == 0)
        {
            Shoot();
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("boundary"))
        {
            //AddReward(-50f);
            //EndEpisode();
        }
    }

    public void Win()
    {
        AddReward(50f);
        EndEpisode();
    }

    public void EnemyDestroyed()
    {
        AddReward(10f);
    }

    private void Update()
    {
        if (cooldown > 0)
            cooldown -= Time.deltaTime;
    }

    void Shoot()
    {
        RaycastHit info;
        if (cooldown > 0)
        {
            return;
        } else
        {
            cooldown = gunCooldownInSeconds;
        }

        if (Physics.Raycast(transform.position, transform.forward, out info, 200f, targetableLayers))
        {
            Debug.Log(info.collider);
            env.RegisterHit(this, info.collider.gameObject.GetComponent<BRAgent>());
            audio.PlayOneShot(killSound);

        } else
        {
            // audio.PlayOneShot(shootSound);
            AddReward(-5f);
        }
    }

}
