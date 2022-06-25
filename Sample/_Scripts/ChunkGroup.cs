using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ChunkGroup : MonoBehaviour
{
    private List<Chunk> _chunks;

    private void Awake()
    {
        _chunks = new List<Chunk>();
    }

    public void Inicializar(List<Vector3> posiciones, Vector3 radio, ObjectPool<GameObject> chunkPool, Transform aSeguir)
    {
        foreach (Vector3 posicion in posiciones)
        {
            GameObject chunkObject = chunkPool.Get();
            chunkObject.transform.parent = transform;

            Chunk chunk = chunkObject.GetComponent<Chunk>();
            _chunks.Add(chunk);

            chunk.Inicializar(posicion, radio);
            chunkObject.GetComponent<LODPorDistancia>().Inicializar(aSeguir);
        }
    }
}
