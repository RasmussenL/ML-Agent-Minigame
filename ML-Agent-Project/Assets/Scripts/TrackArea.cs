using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a collection checkpoints 
/// </summary>
public class TrackArea : MonoBehaviour
{
    // GameObject containing the checkpoints
    private GameObject CheckpointManager;

    // a lookup dictionary for looking up a checkpoint from its collider
    private Dictionary<Collider, Checkpoint> checkpointDictionary;

    /// <summary>
    /// list of all checkpoints in track area
    /// NOTE: checkpoints must be in order in heirarchy.
    /// </summary>
    public List<Checkpoint> Checkpoints { get; private set; }

    /// <summary>
    /// List of starting positions
    /// NOTE: There should be 2 starting positions for the agent
    /// </summary>
    public List<Transform> StartPosistions { get; private set; }

    [Tooltip("Is the player racing the agent?")]
    public bool PlayerVsMLAgent = false;

    /// <summary>
    /// Resets the checkpoints
    /// </summary>
    public void ResetCheckpoints()
    {
        // resets each checkpoint
        foreach (Checkpoint cp in Checkpoints)
        {
            cp.ResetCheckpoint();
            if (PlayerVsMLAgent)
            {
                cp.HideCheckpointBlock();
            }
            else
            {
                cp.ShowCheckpointBlock();
            }
        }
    }

    /// <summary>
    /// Gets the <see cref="Checkpoint"/> that a checkpoint collider belongs to
    /// </summary>
    /// <param name="collider">The checkpoint collider</param>
    /// <returns>The matching Checkpoint</returns>
    public Checkpoint GetCheckpointFromCollider(Collider collider)
    {
        return checkpointDictionary[collider];
    }

    /// <summary>
    /// Recursively find all checkpoints that are children of the transform
    /// </summary>
    /// <param name="parent">Parent transform of the children to check</param>
    private void FindChildCheckpoints(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            // Not the checkpoint manager, look for Checkpoint component
            Checkpoint cp = child.GetComponent<Checkpoint>();
            if (cp != null)
            {
                // Found a checkpoint add it to the Checkpoints list
                Checkpoints.Add(cp);

                // Add collider to dictionary 
                checkpointDictionary.Add(cp.checkpointCollider, cp);

                // A checkpoint should not have any children checkpoints
            }
            else
            {
                // Checkpoint not found, checking children
                FindChildCheckpoints(child);
            }
        }
    }

    /// <summary>
    /// Recursively find all starting positions that are children of the transform
    /// </summary>
    /// <param name="parent">Parent transform of the children to check</param>
    private void FindChildStartingPositions(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            if (child.CompareTag("startPosition"))
            {
                // found a start position
                StartPosistions.Add(child);
            }
            else
            {
                // start position not found, checking children
                FindChildStartingPositions(child);
            }
        }
    }

    /// <summary>
    /// Called when the track wakes up
    /// </summary>
    private void Awake()
    {
        // Initalize variables
        CheckpointManager = new GameObject();
        checkpointDictionary = new Dictionary<Collider, Checkpoint>();
        Checkpoints = new List<Checkpoint>();
        StartPosistions = new List<Transform>();
    }

    /// <summary>
    /// Called when game starts
    /// </summary>
    private void Start()
    {
        // Find all checkpoints within the scene
        FindChildCheckpoints(transform);

        // Find all starting positions in the scene
        FindChildStartingPositions(transform);
    }

}
