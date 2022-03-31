using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{

    public bool enabled = true;
    public float strength = 1f;
    public float minValue = 0f;
    //public AnimationCurve heightCurve;

    public Vector2 offset;
    public int octaves = 5;
    [Range(0, 1)]
    public float persistance = 0.5f;
    [Range(0, 4)]
    public float lacunarity = 2f;
    [Range(0, 1)]
    public float sharpness;

}
