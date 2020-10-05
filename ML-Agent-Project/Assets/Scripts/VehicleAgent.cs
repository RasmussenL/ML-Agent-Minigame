using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Scripting.APIUpdating;

public class VehicleAgent : Agent
{
    [Tooltip("Force to apply when accelerating")]
    public float accelerateForce = 2f;

    [Tooltip("Force to apply when braking")]
    public float brakingForce = 2.5f;

    [Tooltip("Maximun velocity of the agent")]
    public float maximumSpeed = 5f;

    [Tooltip("Rotation speed for turning")]
    public float turningSpeed = 150f;

    [Tooltip("The agent's Camera")]
    public Camera agentCamera;

    [Tooltip("Is this agent in training mode")]
    public bool trainingMode = true;

    [Tooltip("Is this the player?")]
    public bool isPlayer = false;

    [Tooltip("Value that is given as a reward for reaching checkpoints")]
    public float rewardValue = .02f;

    [Tooltip("Value that is given as a punishment for hitting boundaries")]
    public float punishmentValue = -.5f;


    // The Rigidbody of the agent
    new private Rigidbody rigidbody;

    // The track that the agent is in
    private TrackArea trackArea;

    // The next checkpoint for the agent
    private Checkpoint nextCheckpoint;

    // Allows for smoother turning
    private float smoothTurnChange = 0f;

    // Whether the agent is frozen (intentionaly not driving)
    private bool frozen = false;

    /// <summary>
    /// Number of checkpoints the agent has passed
    /// </summary>
    public int CheckpointsPassed { get; private set; }

    /// <summary>
    /// Initialize the agent
    /// </summary>
    public override void Initialize()
    {
        rigidbody = GetComponent<Rigidbody>();
        trackArea = GetComponentInParent<TrackArea>();

        // If not in training mode, no max step, play forever
        if (!trainingMode)
        {
            MaxStep = 0;
        }
    }

    public override void OnEpisodeBegin()
    {
        if (trainingMode)
        {
            trackArea.ResetCheckpoints();
        }

        CheckpointsPassed = 0;

        // Zero out velocities so that movement stops before a new episode begins
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        // Default to spawn in first starting position
        int startPos = Random.Range(0, trackArea.StartPosistions.Count);
        if (trainingMode)
        {
            
        }
        else if (isPlayer)
        {
            startPos = 0;
        }
        else
        {
            startPos = trackArea.StartPosistions.Count - 1;
        }

        // Move the agent to it's starting space
        MoveToStartingPosition(startPos);

        // set the agents next checkpoint
        // NOTE: This should be the first checkpoint in the trackArea.Checkpoints list
        nextCheckpoint = null;
        UpdateNextCheckpoint();
    }







    /// <summary>
    /// Prevent the agent from moving and taking actions
    /// </summary>
    public void FreezeAgent()
    {
        Debug.Assert(trainingMode == false, "Freeze/Unfreeze not supported in training");
        frozen = true;
        rigidbody.Sleep();
    }

    /// <summary>
    /// Resume agent movement and actions
    /// </summary>
    public void UnfreezeAgent()
    {
        Debug.Assert(trainingMode == false, "Freeze/Unfreeze not supported in training");
        frozen = false;
        rigidbody.WakeUp();
    }

    /// <summary>
    /// Moves the agent to a starting position
    /// </summary>
    /// <param name="startingPos"></param>
    private void MoveToStartingPosition(int startingPos)
    {
        transform.position = trackArea.StartPosistions[startingPos].position;
    }

    /// <summary>
    /// Update the next checkpoint for the agent
    /// </summary>
    private void UpdateNextCheckpoint()
    {
        if (nextCheckpoint == null)
        {
            // no next checkpoint set
            nextCheckpoint = trackArea.Checkpoints[0];
        }
        else if (trackArea.Checkpoints.IndexOf(nextCheckpoint) >= trackArea.Checkpoints.Count - 1)
        {
            // End of track loop reached, set back to beginning
            trackArea.ResetCheckpoints();
            nextCheckpoint = trackArea.Checkpoints[0];
        } else
        {
            nextCheckpoint = trackArea.Checkpoints[trackArea.Checkpoints.IndexOf(nextCheckpoint) + 1];
        }
    }

    /// <summary>
    /// Called when the agent's collider enters a trigger collider
    /// </summary>
    /// <param name="collider">The trigger collider</param>
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("checkpoint")){
            Checkpoint cp = trackArea.GetCheckpointFromCollider(collider);
            // hit the next checkpoint
            if (cp.gameObject.name.Equals(nextCheckpoint.gameObject.name))
            {
                if (!isPlayer)
                {
                    cp.HitCheckpoint();
                }
                CheckpointsPassed += 1;

                // Get the next checkpoint to pass through
                UpdateNextCheckpoint();

                if (trainingMode)
                {
                    AddReward(rewardValue);
                }
            }
        }
    }

    /// <summary>
    /// Called when agent collides with something solid
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if(trainingMode && collision.collider.CompareTag("boundary"))
        {
            // Collided with area boundary, negative reward given
            AddReward(punishmentValue);
        }
    }

    private void Update()
    {
        //Drawn line to next collier
        if(nextCheckpoint != null)
        {
            Debug.DrawLine(transform.position, nextCheckpoint.gameObject.transform.position, Color.green);
        }
    }
}
