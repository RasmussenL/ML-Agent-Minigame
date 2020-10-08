using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Scripting.APIUpdating;

public class VehicleAgent : Agent
{
    [Tooltip("Force to apply when accelerating")]
    public float accelerateForce = 2f;

    [Tooltip("Force to apply when braking, Braking force should be a positive number")]
    public float brakingForce = 0.5f;

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

    //Current lap (amount of times checkpoints resets)
    private int currentLap = 1;

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
    /// Called when action is received from either the player input or the nerual network
    /// 
    /// vectorAction[i] represents:
    /// Index 0: acceleration force (range: braking = -1 to accelerating = 1);
    /// Index 1: turning force (range: left = -1 to right = 1)
    /// </summary>
    /// <param name="vectorAction"></param>
    public override void OnActionReceived(float[] vectorAction)
    {
        // If frozen do nothing
        if (frozen) return;

        // Calculate acceleration force
        Vector3 moveForce = Vector3.zero;
        if (vectorAction[0] > 0)
        {
            moveForce = (vectorAction[0] * accelerateForce) * transform.forward;
        }
        else if (vectorAction[0] < 0)
        {
            moveForce = (vectorAction[0] * brakingForce) * transform.forward;
        }

        if (rigidbody.velocity.magnitude < maximumSpeed)
        {
            rigidbody.AddForce(moveForce);
        }

        // Get current rotation
        Vector3 rotationVector = transform.rotation.eulerAngles;

        // Calculate rotation
        float rotationChange = vectorAction[1];

        // Calculate smooth rotation change
        smoothTurnChange = Mathf.MoveTowards(smoothTurnChange, rotationChange, 2f * Time.fixedDeltaTime);

        // Calculate new rotation based on smoothed values
        float newRotation = rotationVector.y + smoothTurnChange * Time.fixedDeltaTime * turningSpeed;

        // Apply the new rotation
        transform.rotation = Quaternion.Euler(0f, newRotation, 0f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // If nextCheckpoint is null, observe an empty array and return early
        if (nextCheckpoint == null)
        {
            sensor.AddObservation(new float[8]);
            return;
        }

        // Observe the agent's local rotation (4 observations)
        sensor.AddObservation(transform.localRotation.normalized);

        // Get a vector from the vechile to the next checkpoint
        Vector3 toCheckpoint = nextCheckpoint.transform.position - transform.position;

        // Observe a normalized vecto pointing to the next checkpoint (3 observations)
        sensor.AddObservation(toCheckpoint.normalized);

        // Observe a dot product that indicates whether the vehicle is pointing toward the checkpoint (1 observation)
        // +1 means that the vehicle is pointing directly at the checkpoint, -1 means directly away
        sensor.AddObservation(Vector3.Dot(transform.forward.normalized, -nextCheckpoint.transform.forward.normalized));

        // 8 total observations
    }



    /// <summary>
    /// When Behavior Type is set to "Heuristic Only" on the agent's Behavior Parameters,
    /// this function will be called. Its return value will be fed into
    /// <see cref="OnActionReceived(float[])"/> instead of using the neural network
    /// </summary>
    /// <param name="actionsOut">An output action array</param>
    public override void Heuristic(float[] actionsOut)
    {
        // Convert controller inputs to movement and turning
        // All values should be between -1 and 1

        // Acceleration/Braking
        float accelerateValue = Input.GetAxis("Gas");

        // Left/Right
        float turnValue = Input.GetAxis("Turn");

        // Add the 2 values to the actionsOut array
        actionsOut[0] = accelerateValue;
        actionsOut[1] = turnValue;

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
            currentLap += 1;
            GameManager.Instance.oCurLap = currentLap;
        }
        else
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
        if (collider.CompareTag("checkpoint"))
        {
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
                    float bonus1 = (rewardValue / 2.0f) * (rigidbody.velocity.magnitude / maximumSpeed);
                    float bonus2 = (rewardValue / 2.0f) * Vector3.Dot((cp.gameObject.transform.position - transform.position), transform.forward);
                    AddReward(rewardValue + bonus1 + bonus2);
                }
            }
        }
        else if (collider.CompareTag("start"))
        {
            if (isPlayer)
            {
                //Debug.Log("Player hit start");
                if (GameManager.Instance.midpointHit)
                {
                    GameManager.Instance.pCurLap += 1;
                    GameManager.Instance.midpointHit = false;
                }
            }
        }
        else if (collider.CompareTag("midpoint"))
        {
            if (isPlayer)
            {
                //Debug.Log("Player hit midpoint");
                GameManager.Instance.midpointHit = true;
            }
        }
    }

    /// <summary>
    /// Called when agent collides with something solid
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (trainingMode && collision.collider.CompareTag("boundary"))
        {
            // Faster collision with boundary = larger negative reward
            float detrement = (punishmentValue / 2.0f) * (rigidbody.velocity.magnitude / maximumSpeed);
            // Collided with area boundary, negative reward given
            AddReward(punishmentValue + detrement);
        }
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        //Drawn line to next collier
        if (nextCheckpoint != null)
        {
            Debug.DrawLine(transform.position, nextCheckpoint.gameObject.transform.position, Color.green);
        }
    }
}
