using UnityEngine;

public class GeneradorPerlin3D : GeneradorDeValores
{
    public override float Valor(float i, float j, float k)
    {
        float ij = Mathf.PerlinNoise(i, j);
        float jk = Mathf.PerlinNoise(j, k);
        float ki = Mathf.PerlinNoise(k, i);

        float ji = Mathf.PerlinNoise(j, k);
        float kj = Mathf.PerlinNoise(k, j);
        float ik = Mathf.PerlinNoise(i, k);

        return (ij + jk + ki + ji + kj + ik) / 6f;
    }
}
