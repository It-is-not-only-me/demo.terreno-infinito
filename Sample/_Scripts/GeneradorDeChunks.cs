using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GeneradorDeChunks : MonoBehaviour
{
    [SerializeField] private Transform _aSeguir;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Vector3Int _tamanioTotal;
    [SerializeField] private Vector3 _radio;
    
    private GameObject[,,] _chunks;

    private void Awake()
    {
        _chunks = new GameObject[_tamanioTotal.x, _tamanioTotal.y, _tamanioTotal.z];
        transform.position = _aSeguir.position;
        CrearMatriz();
    }

    private void Update()
    {
        Vector3 posicion = _aSeguir.position;
        Vector3Int divisiones = Divisiones(posicion);
        for (int i = 0; i < 3; i++)
            posicion[i] = divisiones[i] * _radio[i];

        Vector3Int divisionesDeDiferencias = Divisiones(transform.position - posicion);

        Debug.Log(transform.position - posicion);
        transform.position = posicion;
        if (divisionesDeDiferencias != Vector3Int.zero)
            ActualizarChunks(divisionesDeDiferencias);
    }

    private void CrearMatriz()
    {
        Vector3 esquina = Esquina();

        for (int i = 0; i < _tamanioTotal.x; i++)
            for (int j = 0; j < _tamanioTotal.y; j++)
                for (int k = 0; k < _tamanioTotal.z; k++)
                {
                    Vector3Int posicionLocal = new Vector3Int(i, j, k);
                    _chunks[i, j, k] = GenerarChunk(esquina, posicionLocal);
                }
    }

    private Vector3 Esquina()
    {
        Vector3 esquina = transform.position;
        for (int i = 0; i < 3; i++)
            esquina[i] -= _radio[i] * _tamanioTotal[i];
        return esquina;
    }

    private GameObject GenerarChunk(Vector3 esquina, Vector3Int posicionLocal)
    {
        GameObject chunk = Instantiate(_prefab, transform);
        return GenerarChunk(esquina, posicionLocal, chunk);
    }

    private GameObject GenerarChunk(Vector3 esquina, Vector3Int posicionLocal, GameObject chunk)
    {
        Vector3 posicionChunk = esquina;
        for (int w = 0; w < 3; w++)
            posicionChunk[w] += (_radio[w] * 2) * posicionLocal[w];

        chunk.GetComponent<Chunk>().Inicializar(posicionChunk, _radio);
        chunk.GetComponent<LODPorDistancia>().Inicializar(_aSeguir);
        return chunk;
    }

    private void ActualizarChunks(Vector3Int translacionRelativa)
    {
        Vector3 esquina = Esquina();
        for (int i = 0; i < _tamanioTotal.x; i++)
            for (int j = 0; j < _tamanioTotal.y; j++)
                for (int k = 0; k < _tamanioTotal.z; k++)
                {
                    Vector3Int posicionLocal = new Vector3Int(i, j, k);
                    if (EnRango(posicionLocal, translacionRelativa, _tamanioTotal))
                    {
                        _chunks[i, j, k] = _chunks[i - translacionRelativa.x, j - translacionRelativa.y, k - translacionRelativa.z];
                    }
                    else
                    {
                        _chunks[i, j, k] = GenerarChunk(esquina, posicionLocal, _chunks[i, j, k]);
                    }
                }
    }

    private bool EnRango(Vector3Int posicionLocal, Vector3Int translacionRelativa, Vector3Int maximo)
    {
        for (int i = 0; i < 3; i++)
            if (posicionLocal[i] - translacionRelativa[i] < 0 || maximo[i] <= posicionLocal[i] - translacionRelativa[i])
                return false;
        return true;
    }

    private Vector3Int Divisiones(Vector3 posicion)
    {
        Vector3Int divisiones = Vector3Int.zero;
        for (int i = 0; i < 3; i++)
            divisiones[i] = Mathf.RoundToInt(posicion[i] / (_radio[i]));
        return divisiones;
    }
}