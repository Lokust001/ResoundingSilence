using UnityEngine;

public class PivotCamera : MonoBehaviour
{
    public bool isActive;

    /// <summary>
    /// Allows the script to listen for player input
    /// </summary>
    private void OnEnable()
    {
        InputPublicEvents.MovePressed += PlayerInputStarted;
    }

    /// <summary>
    /// Helper method to update PlayerVelocity based on user input
    /// </summary>
    /// <param name="moveDirection"></param>
    void PlayerInputStarted(Vector2 moveDirection)
    {

    }


}
