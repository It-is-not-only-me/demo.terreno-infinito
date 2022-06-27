using UnityEngine;

public class GeneradorPerlin2D : GeneradorDeValores
{
    [SerializeField] private float _piso = 0;
    [SerializeField] private float _cercania = 10f;
    [SerializeField][Range(1f, 100f)] private float _amplitud = 1;

    public override float Valor(float i, float j, float k)
    {
        float valorPerlin = Mathf.PerlinNoise(i, k) * _amplitud;
        return Mathf.InverseLerp(j - _cercania, j + _cercania, valorPerlin - _piso);
    }
}