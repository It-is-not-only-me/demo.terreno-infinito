using System.Collections.Generic;
using UnityEngine;

public class GeneradorPerlin2D : GeneradorDeValores
{

    [SerializeField] private float _piso = 0;
    [SerializeField] private float _cercania = 10f;
    [SerializeField][Range(1f, 100f)] private float _amplitud = 1;

    [Space]

    [SerializeField] private List<float> _frecuencias = new List<float> { 1f };

    public override float Valor(float i, float j, float k)
    {
        float valorPerlin = 0;

        for (int w = 0; w < _frecuencias.Count; w++)
            valorPerlin += Mathf.PerlinNoise(i * _frecuencias[w], k * _frecuencias[w]) * _amplitud / (w + 1);

        return Mathf.InverseLerp(j - _cercania, j + _cercania, valorPerlin - _piso);
    }
}