using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorUI : MonoBehaviour
{
    public Slider strength;
    public Slider lacunarity;
    public Slider persistance;
    public Slider octaves;
    public Slider sharpness;
    public Slider minValue;
    public Slider offSetX;
    public Slider offSetY;
    public MapGenerator mapGen;
    public Text[] valueText;

    int noiseLayer = 0;
    private void Start()
    {
        mapGen = FindObjectOfType<MapGenerator>();
        UpdateUI();
        UpdateMap();
    }

    private void UpdateUI()
    {
        strength.value = mapGen.noiseSettings[noiseLayer].strength;
        valueText[0].text = mapGen.noiseSettings[noiseLayer].strength.ToString("n2");

        lacunarity.value = mapGen.noiseSettings[noiseLayer].lacunarity;
        valueText[1].text = mapGen.noiseSettings[noiseLayer].lacunarity.ToString("n2");

        persistance.value = mapGen.noiseSettings[noiseLayer].persistance;
        valueText[2].text = mapGen.noiseSettings[noiseLayer].persistance.ToString("n2");

        octaves.value = mapGen.noiseSettings[noiseLayer].octaves;
        valueText[3].text = mapGen.noiseSettings[noiseLayer].octaves.ToString("n2");

        sharpness.value = mapGen.noiseSettings[noiseLayer].sharpness;
        valueText[4].text = mapGen.noiseSettings[noiseLayer].sharpness.ToString("n2");

        minValue.value = mapGen.noiseSettings[noiseLayer].minValue;
        valueText[5].text = mapGen.noiseSettings[noiseLayer].minValue.ToString("n2");

        offSetX.value = mapGen.noiseSettings[noiseLayer].offset.x;
        valueText[6].text = mapGen.noiseSettings[noiseLayer].offset.x.ToString("n2");

        offSetY.value = mapGen.noiseSettings[noiseLayer].offset.y;
        valueText[7].text = mapGen.noiseSettings[noiseLayer].offset.y.ToString("n2");
    }

    public void UpdateMap()
    {
        mapGen.noiseSettings[noiseLayer].strength = strength.value;
        valueText[0].text = mapGen.noiseSettings[noiseLayer].strength.ToString("n2");

        mapGen.noiseSettings[noiseLayer].lacunarity = lacunarity.value;
        valueText[1].text = mapGen.noiseSettings[noiseLayer].lacunarity.ToString("n2");

        mapGen.noiseSettings[noiseLayer].persistance = persistance.value;
        valueText[2].text = mapGen.noiseSettings[noiseLayer].persistance.ToString("n2");

        mapGen.noiseSettings[noiseLayer].octaves = (int)octaves.value;
        valueText[3].text = mapGen.noiseSettings[noiseLayer].octaves.ToString("n2");

        mapGen.noiseSettings[noiseLayer].sharpness = sharpness.value;
        valueText[4].text = mapGen.noiseSettings[noiseLayer].sharpness.ToString("n2");

        mapGen.noiseSettings[noiseLayer].minValue = minValue.value;
        valueText[5].text = mapGen.noiseSettings[noiseLayer].minValue.ToString("n2");

        mapGen.noiseSettings[noiseLayer].offset.x = offSetX.value;
        valueText[6].text = mapGen.noiseSettings[noiseLayer].offset.x.ToString("n2");

        mapGen.noiseSettings[noiseLayer].offset.y = offSetY.value;
        valueText[7].text = mapGen.noiseSettings[noiseLayer].offset.y.ToString("n2");
        mapGen.RegenerateWorld();
    }
    public void UpdateNoiseSelection(int number)
    {
        noiseLayer = number;
        UpdateUI();

    }

    private void Update()
    {
        
    }
}
