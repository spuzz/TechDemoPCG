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

    private void Start()
    {
        mapGen = FindObjectOfType<MapGenerator>();
        strength.value = mapGen.noiseSettings[0].strength;
        lacunarity.value = mapGen.noiseSettings[0].lacunarity;
        persistance.value = mapGen.noiseSettings[0].persistance;
        octaves.value = mapGen.noiseSettings[0].octaves;
        sharpness.value = mapGen.noiseSettings[0].sharpness;
        minValue.value = mapGen.noiseSettings[0].minValue;
        offSetX.value = mapGen.noiseSettings[0].offset.x;
        offSetY.value = mapGen.noiseSettings[0].offset.y;
        UpdateMap();
    }

    public void UpdateMap()
    {
        mapGen.noiseSettings[0].strength = strength.value;
        valueText[0].text = mapGen.noiseSettings[0].strength.ToString();

        mapGen.noiseSettings[0].lacunarity = lacunarity.value;
        valueText[1].text = mapGen.noiseSettings[0].lacunarity.ToString();

        mapGen.noiseSettings[0].persistance = persistance.value;
        valueText[2].text = mapGen.noiseSettings[0].persistance.ToString();

        mapGen.noiseSettings[0].octaves = (int)octaves.value;
        valueText[3].text = mapGen.noiseSettings[0].octaves.ToString();

        mapGen.noiseSettings[0].sharpness = sharpness.value;
        valueText[4].text = mapGen.noiseSettings[0].sharpness.ToString();

        mapGen.noiseSettings[0].minValue = minValue.value;
        valueText[5].text = mapGen.noiseSettings[0].minValue.ToString();

        mapGen.noiseSettings[0].offset.x = offSetX.value;
        valueText[6].text = mapGen.noiseSettings[0].offset.x.ToString();

        mapGen.noiseSettings[0].offset.y = offSetY.value;
        valueText[7].text = mapGen.noiseSettings[0].offset.y.ToString();
        mapGen.RegenerateWorld();
    }

    private void Update()
    {
        
    }
}
