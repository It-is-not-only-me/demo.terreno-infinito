using UnityEngine;

public class GeneradorPerlin2D : GeneradorDeValores
{
    [SerializeField] private float _piso = 0;
    [SerializeField][Range(1f, 100f)] private float _amplitud = 1;

    public override float Valor(float i, float j, float k)
    {
        if (j < _piso)
            return 0;

        return Mathf.PerlinNoise(i, k) * _amplitud * Mathf.Abs(j - _piso); 
    }
}