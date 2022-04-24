using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{

    public bool enabled = true;

    // Layer impact
    public float strength = 1f;

    // Minimum height that will take affect
    public float minValue = 0f;

    // Level of perlin noise detail
    [Range(10, 200)]
    public float scale = 50;

    public Vector2 offset;

    // number of octave for noise layer FBM function
    public int octaves = 5;

    // determines amplitude of each octave
    [Range(0, 1)]
    public float persistance = 0.5f;

    // determines frequency of each octave
    [Range(0, 4)]
    public float lacunarity = 2f;

    // Combined billow/ridge noise
    [Range(-1, 1)]
    public float sharpness;

    // Warp noise by feeding in additional layers of FBM into coords
    public Vector2 warp1;
    public Vector2 warp2;
    [Range(0, 5)]
    public float warpScale;
}
