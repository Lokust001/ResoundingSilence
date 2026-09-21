/*
* Author: Tyler
* Contributors: Brad Dixon
* Last Modified: 09/18/2026
* Summary: Reads in all the player's inputs and throws them through public events
* To Do:   Add more player inputs as needed.
*/


using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

//needs the playerinput to function
[RequireComponent (typeof(PlayerInput))]
public class InputManager : BaseManager
{
    public static InputManager Instance;

    #region InputActions

    private PlayerInput pInput;
    private InputAction move;

    private InputAction shoot;
    private InputAction aim;

    private InputAction abilityOne;
    private InputAction abilityTwo;

    private InputAction interact;

    #endregion

    #region Setup
    /// <summary>
    /// initializes the player inputs
    /// </summary>
    /// <returns></returns>
    public async override Awaitable InitManager()
    {
        await base.InitManager();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        await SetUpInputActions();
    }

    /// <summary>
    /// Performs first time set up for the input actions
    /// </summary>
    /// <returns></returns>
    private async Awaitable SetUpInputActions()
    {
        //grabs the player input and enables the map
        pInput = GetComponent<PlayerInput>();
        pInput.currentActionMap.Enable();


        //finds all of the input actions we are using
        move = pInput.currentActionMap.FindAction("Move");
        shoot = pInput.currentActionMap.FindAction("Shoot");
        interact = pInput.currentActionMap.FindAction("Interact");
        aim = pInput.currentActionMap.FindAction("Aim");

        //sets up the individual input actions
        move.performed += Move_performed;
        move.canceled += Move_canceled;

        shoot.started += Shoot_started;
        shoot.canceled += Shoot_canceled;

        interact.started += Interact_started;
        interact.canceled += Interact_canceled;

        aim.performed += Aim_performed;

        abilityOne.started += AbilityOne_started;
        abilityOne.canceled += AbilityOne_canceled;

        abilityTwo.started += AbilityTwo_started;
        abilityTwo.canceled += AbilityTwo_canceled;

        await Task.CompletedTask;
    }

    #endregion

    #region InputHandling Functions

    /// <summary>
    /// Throws the movepressed public event
    /// </summary>
    /// <param name="obj"></param>
    private void Move_performed(InputAction.CallbackContext obj)
    {
        InputPublicEvents.MovePressed?.Invoke(obj.ReadValue<Vector2>());
    }

    /// <summary>
    /// Throws the movecancelled public event
    /// </summary>
    /// <param name="obj"></param>
    private void Move_canceled(InputAction.CallbackContext obj)
    {
        InputPublicEvents.MoveReleased?.Invoke();
    }

    /// <summary>
    /// Throws the shootpressed public event
    /// </summary>
    /// <param name="obj"></param>
    private void Shoot_started(InputAction.CallbackContext obj)
    {
        InputPublicEvents.ShootPressed?.Invoke();
    }

    /// <summary>
    /// Throws the shootcancelled public event
    /// </summary>
    /// <param name="obj"></param>
    private void Shoot_canceled(InputAction.CallbackContext obj)
    {
        InputPublicEvents.ShootReleased?.Invoke();
    }

    /// <summary>
    /// Throws the interactpressed public event
    /// </summary>
    /// <param name="obj"></param>
    private void Interact_started(InputAction.CallbackContext obj)
    {
        InputPublicEvents.InteractPressed?.Invoke();
    }

    /// <summary>
    /// Throws the interactcancelled public event
    /// </summary>
    /// <param name="obj"></param>
    private void Interact_canceled(InputAction.CallbackContext obj)
    {
        InputPublicEvents.InteractReleased?.Invoke();
    }

    /// <summary>
    /// Calls the public event that returns the mouse's position
    /// </summary>
    /// <param name="obj"></param>
    private void Aim_performed(InputAction.CallbackContext obj)
    {
        InputPublicEvents.MouseMoved?.Invoke(obj.ReadValue<Vector2>());
    }

    /// <summary>
    /// Calls the public event for pressing the ability one button
    /// </summary>
    /// <param name="obj"></param>
    private void AbilityOne_started(InputAction.CallbackContext obj)
    {
        InputPublicEvents.AbilityOnePressed?.Invoke();
    }

    /// <summary>
    /// Calls the public event for releasing the ability one button
    /// </summary>
    /// <param name="obj"></param>
    private void AbilityOne_canceled(InputAction.CallbackContext obj)
    {
        InputPublicEvents.AbilityOneReleased?.Invoke();
    }

    /// <summary>
    /// Calls the public event for pressing the ability two button
    /// </summary>
    /// <param name="obj"></param>
    private void AbilityTwo_started(InputAction.CallbackContext obj)
    {
        InputPublicEvents.AbilityTwoPressed?.Invoke();
    }

    /// <summary>
    /// Calls the public event for releasing the ability two button
    /// </summary>
    /// <param name="obj"></param>
    private void AbilityTwo_canceled(InputAction.CallbackContext obj)
    {
        InputPublicEvents.AbilityTwoReleased?.Invoke();
    }

    #endregion
}
