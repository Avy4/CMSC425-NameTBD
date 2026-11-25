using UnityEngine;

public class PlayerDeath : Death
{
    [SerializeField] RecordPhantom recordPhantom;
    [SerializeField] PlayerMovement playerMovement;

    // Everytime the player dies a phantom must be created up to that point
    // Player also needs to be reset, but create phantom takes care of that
    public override void Die()
    {   
        recordPhantom.CreatePhantom();
        // Debug.Log("Create Phantom (Die)");
        playerMovement.ResetPlayer();
        // Debug.Log("Reset Player (Die)");

        // Fresh Line Renderer
        // We want to only reset the line renderer (and start reading position again after resetting pos)
        recordPhantom.ResetLineRenderer();
        // Debug.Log("Reset LineRenderer (Die)");

        PushableBoxes.AlertResetBlock();
    }
}
