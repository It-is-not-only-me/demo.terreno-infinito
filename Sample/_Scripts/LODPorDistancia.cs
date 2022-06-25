using System.Collections.Generic;
using UnityEngine;

public class LODPorDistancia : DeterminarLOD
{
    [SerializeField] private ConfiguracionLOD _configuracionLOD;

    private Transform _aSeguir;

    public void Inicializar(Transform aSeguir)
    {
        _aSeguir = aSeguir;
    }

    public override int LOD(Vector3 posicion)
    {
        float distanciaActual = Vector3.Distance(posicion, _aSeguir.position);
        return (int) _configuracionLOD.DistanciaMaximaPorLOD(distanciaActual);
    }
}


