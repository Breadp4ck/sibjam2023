using Godot;
using System;

public static partial class Timescale
{
    public static float Scale => _scale;
    private static float _scale = 1.0f;
    
    public static void SetTimescale(float scale)
    {
        _scale = scale;
    }
}
