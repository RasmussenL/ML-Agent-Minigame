using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a single checkpoint
/// </summary>
public class Checkpoint : MonoBehaviour
{
    [Tooltip("The color when checkpoint has not been hit")]
    public Color unhitCheckpointColor = Color.red;

    [Tooltip("The color when checkpoint has been hit")]
    public Color hitCheckpointColor = Color.blue;

    /// <summary>
    /// The Trigger collider representing the checkpoint
    /// </summary>
    [HideInInspector]
    public Collider checkpointCollider;

    private Material checkpointMaterial;

    /// <summary>
    /// Whether or not the agent has passed through the checkpoint
    /// </summary>
    public bool HasCheckpointBeenHit { get; private set; }

    /// <summary>
    /// Called when checkpoint is hit by agent
    /// </summary>
    public void HitCheckpoint()
    {
        // Deactivate checkpoint
        checkpointCollider.gameObject.SetActive(false);
        HasCheckpointBeenHit = true;

        // set checkpoint block color to indicate that it has been hit
        checkpointMaterial.SetColor("_BaseColor", hitCheckpointColor);
    }

    /// <summary>
    /// Resets the Checkpoint
    /// </summary>
    public void ResetCheckpoint()
    {
        // reset HasCheckpointBeenHit
        HasCheckpointBeenHit = false;

        // Enable collider
        checkpointCollider.gameObject.SetActive(true);

        // change collider material to indicate that it can be hit
        checkpointMaterial.SetColor("_BaseColor", unhitCheckpointColor);

    }

    /// <summary>
    /// Hides the marker for the checkpoint
    /// </summary>
    public void HideCheckpointBlock()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    /// <summary>
    /// Displays the marker for the checkpoint;
    /// </summary>
    public void ShowCheckpointBlock()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = true;
    }

    /// <summary>
    /// Called when checkpoint wakes up
    /// </summary>
    private void Awake()
    {
        // Find the main mesh renderer for the checkpoint and get its material
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        checkpointMaterial = meshRenderer.material;

        // Find the checkpoint collider
        checkpointCollider = transform.Find("checkpointCollider").GetComponent<Collider>();
    }
}
