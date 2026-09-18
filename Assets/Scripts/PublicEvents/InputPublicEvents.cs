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
    public static Action<Vector2> MouseMoved;

    public static Action<Vector2> MovePressed;
    public static Action MoveReleased;

    public static Action ShootPressed;
    public static Action ShootReleased;

    public static Action InteractPressed;
    public static Action InteractReleased;
}
