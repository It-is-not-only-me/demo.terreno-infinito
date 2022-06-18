using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "configuracion LOD", menuName = "MarchingCubes/Demo/Configuracion LOD")]
public class ConfiguracionLOD : ScriptableObject
{
    public List<Vector3Int> numeroDePuntosPorNivelDeDetalle;
}
