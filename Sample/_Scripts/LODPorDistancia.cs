using System.Collections.Generic;
using UnityEngine;

public class LODPorDistancia : DeterminarLOD
{
    [SerializeField] private Transform _transform;
    [SerializeField] private List<float> _intervalosDeDistancias;

    public override int LOD(Vector3 posicion)
    {
        float distanciaActual = Vector3.Distance(posicion, _transform.position);
        int lod = 0;
        foreach (float distanciaMaxima in _intervalosDeDistancias)
        {
            if (distanciaActual < distanciaMaxima)
                break;
            lod++;
        }
        return lod;
    }
}


