using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "configuracion LOD", menuName = "MarchingCubes/Demo/Configuracion LOD")]
public class ConfiguracionLOD : ScriptableObject
{
    [System.Serializable]
    private struct ConfiguracionLODEspecifico
    {
        [SerializeField] public Vector3Int NumeroDePuntosPorNivel;
        [SerializeField] public float DistanciaMaxima;
    }

    [SerializeField] private List<ConfiguracionLODEspecifico> _lodDetalles;

    public Vector3Int NumeroDePuntosPorLOD(uint lod)
    {
        return _lodDetalles[(int)lod].NumeroDePuntosPorNivel;
    }

    public uint DistanciaMaximaPorLOD(float distancia)
    {
        uint lod = 0;
        for (int i = 0; i < _lodDetalles.Count - 1; i++)
        {
            if (distancia < _lodDetalles[i].DistanciaMaxima)
                break;
            lod++;
        }
        return lod;
    }
}
