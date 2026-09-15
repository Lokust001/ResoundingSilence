/* Author: Dalsten Yan
 * Creation Date 9/15/2026
 * 
 * PlayerController.cs enables the player character to move based on captured PlayerInput 
 */

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float moveSpeed;

    #region Private Variables
    Rigidbody rigidbody;
    Vector3 playerVelocity;
    #endregion


    #region Unity Methods
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        //Remove later, use now for debug
        playerVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    #endregion

    void MovePlayer() 
    {
        //Capture Player Movement from InputHandler
        
        //Propagate calculated velocity to rigidbody & make player move
        rigidbody.linearVelocity = playerVelocity * moveSpeed;
    }
}
