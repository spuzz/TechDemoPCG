using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{

    public bool enabled = true;
    public float strength = 1f;
    public float minValue = 0f;
    [Range(10, 200)]
    public float scale = 50;
    //public AnimationCurve heightCurve;

    public Vector2 offset;
    public int octaves = 5;
    [Range(0, 1)]
    public float persistance = 0.5f;
    [Range(0, 4)]
    public float lacunarity = 2f;
    [Range(-1, 1)]
    public float sharpness;

    public Vector2 warp1;
    public Vector2 warp2;
    [Range(0, 5)]
    public float warpScale;
}
