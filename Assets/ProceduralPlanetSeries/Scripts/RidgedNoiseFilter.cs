using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidgedNoiseFilter: INoiseFilter
{
    private Noise noise = new Noise();
    private NoiseSettings.RidgedNoiseSettings settings;

    public RidgedNoiseFilter(NoiseSettings.RidgedNoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1;

        // We use a weight so we get the ridges to be much more detailed than the valleys
        float weight = 1;

        for (int i = 0; i < settings.numLayers; i++)
        {
            /*
             * To get sharp ridges, visualize the noise value as a sin wave
             * i.e. values between -1 to 1. Taking the abs value of this wave,
             * we get sharp valleys. Then we invert this wave by subtracting from
             * 1 to get sharp peaks, aka 'ridges'
             */ 
            float v = 1 - Mathf.Abs(noise.Evaluate(point * frequency + settings.center));

            // For more pronounced ridges, simply raise the value to any power, eg: v^2
            v *= v;

            // Factor in the weight
            v *= weight;
            weight = Mathf.Clamp01(v * settings.weightMultiplier);

            // Since we already have a value between 0-1
            // we do NOT need to +1 and *0.5 like in the
            // SimpleNoiseFilter
            noiseValue += v * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistence;
        }

        noiseValue = noiseValue - settings.minValue;
        return noiseValue * settings.strength;
    }
}
