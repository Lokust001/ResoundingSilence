using System;
using UnityEngine;

public static class InputPublicEvents
{
    public static Action<Vector2> MovePressed;
    public static Action MoveReleased;

    public static Action ShootPressed;
    public static Action ShootReleased;

    public static Action InteractPressed;
    public static Action InteractReleased;
}
