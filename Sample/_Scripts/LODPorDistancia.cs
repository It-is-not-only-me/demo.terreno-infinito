using System.Collections.Generic;
using UnityEngine;

public class LODPorDistancia : DeterminarLOD
{
    [SerializeField] private List<float> _intervalosDeDistancias;

    private Transform _aSeguir;

    public void Inicializar(Transform aSeguir)
    {
        _aSeguir = aSeguir;
    }

    public override int LOD(Vector3 posicion)
    {
        float distanciaActual = Vector3.Distance(posicion, _aSeguir.position);
        int lod = 0;
        for (int i = 0; i < _intervalosDeDistancias.Count - 1; i++)
        {
            if (distanciaActual < _intervalosDeDistancias[i])
                break;
            lod++;
        }
        return lod;
    }
}


