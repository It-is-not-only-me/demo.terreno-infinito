using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GeneradorDeChunks : MonoBehaviour
{
    private static Vector3Int _tamanioMaximo = new Vector3Int(100, 100, 100);

    [SerializeField] private Transform _aSeguir;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Vector3Int _tamanioTotal;
    [SerializeField] private Vector3 _radio;
    
    private GameObject[,,] _chunks;
    private Vector3Int _tamanioPrevio;

    private ObjectPool<GameObject> _chunksPool;
    private Vector3 PosicionASeguir => _aSeguir.position;


    // no usar una matriz

    private void Awake()
    {
        _chunks = new GameObject[_tamanioTotal.x, _tamanioTotal.y, _tamanioTotal.z];
        ActualizarTamanio();

        Vector3Int cantidadDefualt = Vector3Int.Min(_tamanioTotal * 3 / 2, _tamanioMaximo);
        

        _chunksPool = new ObjectPool<GameObject>(
            () => Instantiate(_prefab, transform),
            (chunk) => chunk.SetActive(true),
            (chunk) => chunk.SetActive(false),
            (chunk) => Destroy(chunk),
            false,
            _tamanioTotal.x * _tamanioTotal.y * _tamanioTotal.z,
            _tamanioMaximo.x * _tamanioMaximo.y * _tamanioMaximo.z
        );

        transform.position = NuevaPosicion();
        RellenarMatriz(_chunks, _tamanioTotal);
    }

    private void Update()
    {
        Vector3 posicion = NuevaPosicion();
        Vector3Int divisionesDeDiferencias = Divisiones(transform.position - posicion);
        transform.position = posicion;

        if (_tamanioTotal.x == 0 || _tamanioTotal.y == 0 || _tamanioTotal.z == 0)
            _tamanioTotal = _tamanioPrevio;

        if (divisionesDeDiferencias != Vector3Int.zero || _tamanioTotal != _tamanioPrevio)
            ActualizarChunks(divisionesDeDiferencias);

        ActualizarTamanio();
    }

    private void ActualizarTamanio()
    {
        _tamanioTotal = Vector3Int.Min(_tamanioTotal, _tamanioMaximo);
        _tamanioPrevio = _tamanioTotal;
    }

    private Vector3 NuevaPosicion()
    {
        Vector3 posicion = PosicionASeguir;
        Vector3Int divisiones = Divisiones(posicion);
        for (int i = 0; i < 3; i++)
            posicion[i] = divisiones[i] * _radio[i] * 2;
        return posicion;
    }

    private void ActualizarChunks(Vector3Int translacionRelativa)
    {
        GameObject[,,] nuevaIteracion = new GameObject[_tamanioTotal.x, _tamanioTotal.y, _tamanioTotal.z];
        translacionRelativa *= -1;

        for (int i = 0; i < _tamanioPrevio.x; i++)
            for (int j = 0; j < _tamanioPrevio.y; j++)
                for (int k = 0; k < _tamanioPrevio.z; k++)
                {
                    Vector3Int posicionLocal = new Vector3Int(i, j, k);
                    if (EnRango(posicionLocal, translacionRelativa, _tamanioTotal))
                    {
                        nuevaIteracion[i - translacionRelativa.x, j - translacionRelativa.y, k - translacionRelativa.z] = _chunks[i, j, k];
                        _chunks[i, j, k] = null;
                    }
                }

        for (int i = 0; i < _tamanioPrevio.x; i++)
            for (int j = 0; j < _tamanioPrevio.y; j++)
                for (int k = 0; k < _tamanioPrevio.z; k++)
                    if (_chunks[i, j, k] != null)
                        _chunksPool.Release(_chunks[i, j, k]);

        RellenarMatriz(nuevaIteracion, _tamanioTotal);
        _chunks = nuevaIteracion;
    }

    private void RellenarMatriz(GameObject[,,] chunks, Vector3Int tamanio)
    {
        Vector3 posicion = transform.position;
        for (int i = 0; i < 3; i++)
            posicion[i] -= (_radio[i] * 2) * (tamanio[i] / 2); 

        for (int i = 0; i < tamanio.x; i++)
            for (int j = 0; j < tamanio.y; j++)
                for (int k = 0; k < tamanio.z; k++)
                    if (chunks[i, j, k] == null)
                    {
                        Vector3Int posicionLocal = new Vector3Int(i, j, k);
                        chunks[i, j, k] = GenerarChunk(posicion, posicionLocal);
                    }
    }

    private GameObject GenerarChunk(Vector3 esquina, Vector3Int posicionLocal)
    {
        GameObject chunk = _chunksPool.Get();
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
            divisiones[i] = Mathf.RoundToInt((posicion[i]) / _radio[i]);
        return divisiones;
    }
}