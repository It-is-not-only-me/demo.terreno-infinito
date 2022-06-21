using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "configuracion LOD", menuName = "MarchingCubes/Demo/Configuracion LOD")]
public class ConfiguracionLOD : ScriptableObject
{
    [SerializeField] private List<Vector3Int> _numeroDePuntosPorNivelDeDetalle;

    public Vector3Int NumeroDePuntosPorLOD(uint lod)
    {
        return _numeroDePuntosPorNivelDeDetalle[(int)lod];
    }
}
