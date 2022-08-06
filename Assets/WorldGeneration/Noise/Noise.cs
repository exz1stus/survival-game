using UnityEngine;

[CreateAssetMenu(fileName = "New Noise", menuName = "World Generation/New Noise")]
public class Noise : ScriptableObject
{
    public int seed = 100;

    [Range(1, 10000)] public float scale = 30.0F;

    [Range(1, 5)] public int octaves = 1;

    [Range(0, 1)] public float persistence = 0.5F;

    public float lacunarity = 1.0F;
}
