using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : GameInterface
{
    public bool Up { get; private set; }
    public bool Down { get; private set; }
    public bool Left { get; private set; }
    public bool Right { get; private set; }
    public bool Space { get; private set; }
    public bool C { get; private set; }
    public bool Shift { get; private set; }
    public float[,] rotateAngel = new float[3, 3]
    {
        { -45f , 0f , 45f},
        { -90f , -999f , 90f},
        { -135f , 180f , 135f},
    };
    public override void StartUp()
    {
        base.StartUp();
    }
    public override void UpdateData()
    {
        base.UpdateData();

        Up = Input.GetKey(KeyCode.W);
        Down = Input.GetKey(KeyCode.S);
        Left = Input.GetKey(KeyCode.A);
        Right = Input.GetKey(KeyCode.D);
        Space = Input.GetKey(KeyCode.Space);
        C = Input.GetKey(KeyCode.C);
        Shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }
    public float GetAngle()
    {
        int x = 1;
        int y = 1;

        if (Up) x--;
        if (Down) x++;
        if (Left) y--;
        if (Right) y++;

        return rotateAngel[x, y];
    }
    public override void Close()
    {
        base.Close();
    }
}
