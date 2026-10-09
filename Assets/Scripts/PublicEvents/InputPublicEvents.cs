/*
* Author: Tyler
* Contributors: Brad Dixon
* Last Modified: 09/18/2026
* Summary: Stores the public events for the player's inputs
* To Do:   Add more player inputs as needed.
*/


using System;
using UnityEngine;

public static class InputPublicEvents
{
    public static Action ControllerEnabled;
    public static Action KeyboardMouseEnabled;

    public static Action ToggleDebugConsole;

    #region MidRun
    public static Action<Vector2> MouseMoved;
    public static Action<Vector2> PlayerAimed;

    public static Action<Vector2> MovePressed;
    public static Action MoveReleased;

    public static Action ShootPressed;
    public static Action ShootReleased;

    public static Action InteractPressed;
    public static Action InteractReleased;

    public static Action AbilityOnePressed;

    public static Action AbilityTwoPressed;

    public static Action DashPressed;

    public static Action ToggleUpgradeMenuPressed;

    public static Action PausePressed;

    public static Action SwapWeaponPressed;

    #endregion

    #region UpgradeMenu Controls

    public static Action SelectPin;

    public static Action PinReleased;

    public static Action EnableWeapon1;
    public static Action EnableWeapon2;
    public static Action SwapFocusToTarot;
    public static Action SwapFocusToInventory;
    public static Action SwapFocusToGrid;

    public static Action AimCancelled;

    public static Action DropPin;

    #endregion
}
