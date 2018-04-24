using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise
{
    private long seed;

    public PerlinNoise(long seed)
    {
        this.seed = seed;
    }

    private int Random(long x, int range)
    {
        return (int)(((x + seed) ^ 5) % range);
    }

    public int GetNoise(int x, int range)
    {
        float noise = 0;
        int chunkSize = 128;
        range /= 2;

        while (chunkSize > 0)
        {
            int chunkIndex = x / chunkSize;
            float p = (x % chunkSize) / (chunkSize * 1f);
            float right = Random(chunkIndex, range);
            float left = Random(chunkIndex + 1, range);
            
            noise += (1 - p) * left + p * right;
            chunkSize /= 2;
            range /= 2;
            range = Mathf.Max(1, range);
        }

        return (int)Mathf.Round(noise);
    }
}
