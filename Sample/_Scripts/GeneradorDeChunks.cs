using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

public class GeneradorDeChunks : MonoBehaviour
{
    private static Vector3Int _tamanioMaximo = new Vector3Int(100, 100, 100);

    [SerializeField] private Transform _aSeguir;
    [SerializeField] private GameObject _chunkPrefab;
    [SerializeField] private Vector3Int _tamanioTotal;
    [SerializeField] private Vector3 _radio;

    [Space]

    [SerializeField] int _cantidadDeChunks = 10;


    private ObjectPool<GameObject> _chunksPool;
    private Vector3 PosicionASeguir => _aSeguir.position;

    private List<GameObject> _chunksList;
    private Vector3 _posicionAnterior;

    private void Awake()
    {
        ActualizarTamanio();
        Vector3Int cantidadDefualt = Vector3Int.Min(_tamanioTotal * 3 / 2, _tamanioMaximo);

        transform.position = NuevaPosicion();
        _chunksPool = new ObjectPool<GameObject>(
            () => Instantiate(_chunkPrefab),
            (chunk) => chunk.SetActive(true),
            (chunk) => chunk.SetActive(false),
            (chunk) => Destroy(chunk),
            false,
            _tamanioTotal.x * _tamanioTotal.y * _tamanioTotal.z,
            _tamanioMaximo.x * _tamanioMaximo.y * _tamanioMaximo.z
        );

        _chunksList = new List<GameObject>();
        _posicionAnterior = NuevaPosicion();
        ActualizarEspacio(_tamanioTotal);
    }

    private void ActualizarTamanio()
    {
        _tamanioTotal = Vector3Int.Min(_tamanioTotal, _tamanioMaximo);
    }

    private void Update()
    {
        Vector3 nuevaPosicion = NuevaPosicion();
        if (_posicionAnterior == nuevaPosicion)
            return;

        _posicionAnterior = nuevaPosicion;
        ActualizarTamanio();
        ActualizarEspacio(_tamanioTotal);
    }

    private Vector3 NuevaPosicion()
    {
        Vector3 posicion = PosicionASeguir;
        Vector3Int divisiones = Divisiones(posicion);
        for (int i = 0; i < 3; i++)
            posicion[i] = divisiones[i] * _radio[i] * 2;
        return posicion;
    }

    private async void ActualizarEspacio(Vector3Int tamanio)
    {
        Vector3 posicion = NuevaPosicion();
        List<Vector3> posicionesDeChunks = PosicionesDisponibles(posicion, tamanio);
        List<Vector3> posicionesNuevas = new List<Vector3>();

        // eliminando chunks no necesarios
        List<GameObject> chunksAEliminar = new List<GameObject>();
        foreach (GameObject chunk in _chunksList)
        {
            bool existePosicion = false;
            foreach (Vector3 posicionChunk in posicionesDeChunks)
                existePosicion |= PosicionesIguales(chunk, posicionChunk);
            if (!existePosicion)
                chunksAEliminar.Add(chunk);
        }

        chunksAEliminar.ForEach(chunk =>
        {
            _chunksList.Remove(chunk);
            _chunksPool.Release(chunk);
        });

        // posiciones posibles
        foreach (Vector3 posicionChunk in posicionesDeChunks)
        {
            bool existeChunk = false;
            foreach (GameObject chunk in _chunksList)
                existeChunk |= PosicionesIguales(chunk, posicionChunk);
            if (!existeChunk)
                posicionesNuevas.Add(posicionChunk);
        }

        int contador = 0;
        foreach (Vector3 posicionChunk in posicionesNuevas)
        {
            _chunksList.Add(CrearChunk(posicionChunk));
            contador++;

            if (contador % _cantidadDeChunks == 0)
                await Task.Yield();
        }
    }

    private bool PosicionesIguales(GameObject chunk, Vector3 posicion)
    {
        bool iguales = true;
        float epsilon = 0.01f;
        Vector3 posicionChunk = chunk.transform.position;

        for (int i = 0; i < 3; i++)
            iguales &= posicion[i] - epsilon < posicionChunk[i] && posicionChunk[i] < posicion[i] + epsilon;

        return iguales;
    }

    private GameObject CrearChunk(Vector3 posicion)
    {
        GameObject chunkObject = _chunksPool.Get();
        chunkObject.transform.parent = transform;

        Chunk chunk = chunkObject.GetComponent<Chunk>();
        chunk.Inicializar(posicion, _radio);
        chunkObject.GetComponent<LODPorDistancia>().Inicializar(_aSeguir);

        return chunkObject;
    }

    private List<Vector3> PosicionesDisponibles(Vector3 centro, Vector3Int tamanio)
    {
        Vector3 esquina = Vector3.zero;
        for (int i = 0; i < 3; i++)
            esquina[i] -= (_radio[i] * 2) * (tamanio[i] / 2);
        esquina += centro;

        List<Vector3> posicionesDisponibles = new List<Vector3>();

        for (int i = 0; i < tamanio.x; i++)
            for (int j = 0; j < tamanio.y; j++)
                for (int k = 0; k < tamanio.z; k++)
                    posicionesDisponibles.Add(PosicionChunk(esquina, i, j, k));

        return OrdenarPorDistanciaAlCentro(posicionesDisponibles, centro);
    }

    private List<Vector3> OrdenarPorDistanciaAlCentro(List<Vector3> posiciones, Vector3 centro)
    {
        for (int i = 0; i < posiciones.Count; i++)
        {
            float primeraDistancia = Vector3.Distance(centro, posiciones[i]);
            for (int j = i + 1; j < posiciones.Count; j++)
            {
                float segundaDistancia = Vector3.Distance(centro, posiciones[j]);
                if (primeraDistancia < segundaDistancia)
                {
                    Vector3 auxilear = posiciones[i];
                    posiciones[i] = posiciones[j];
                    posiciones[j] = auxilear;
                }
            }
        }
        posiciones.Reverse();
        return posiciones;
    }

    private Vector3 PosicionChunk(Vector3 esquina, int i, int j, int k)
    {
        Vector3Int posicionLocal = new Vector3Int(i, j, k);
        Vector3 posicionChunk = esquina;
        for (int w = 0; w < 3; w++)
            posicionChunk[w] += (_radio[w] * 2) * posicionLocal[w];
        return posicionChunk;
    }

    private Vector3Int Divisiones(Vector3 posicion)
    {
        Vector3Int divisiones = Vector3Int.zero;
        for (int i = 0; i < 3; i++)
            divisiones[i] = Mathf.RoundToInt((posicion[i]) / _radio[i]);
        return divisiones;
    }
}