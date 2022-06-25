using System.Collections;
using UnityEngine;
using ItIsNotOnlyMe.MarchingCubes;

/*
 * La intencion de esta clase que sin importar el nivel de detalle
 * que sea capaz de representar bien su contendio usando marching cubes
 */
public class Chunk : GenerarDatos
{
    public override Bounds Bounds => _bounds;

    public override Vector3Int NumeroDePuntosPorEje => _configuracionLOD.NumeroDePuntosPorLOD(LOD());

    public override bool Actualizar
    {
        get
        {
            bool actualizarse = _actualizar;
            _actualizar &= false;
            return actualizarse;
        }
    }

    [SerializeField] private ConfiguracionLOD _configuracionLOD;

    private DeterminarLOD _determinarLOD;
    private Vector3 _radio;
    private Bounds _bounds;
    private int _lodAnterior;
    private bool _actualizar;

    private void Awake()
    {
        if (!TryGetComponent(out _determinarLOD))
            Debug.LogError("Se necesita una forma de determinar el nivel de detalle");
    }

    public void Inicializar(Vector3 posicion, Vector3 radio)
    {
        _radio = radio;
        transform.position = posicion;
        _lodAnterior = -1;
        _actualizar = true;
        _bounds = new Bounds(posicion, _radio * 2);
    }

    private uint LOD()
    {
        int lodActual = _determinarLOD.LOD(transform.position);
        ActualizarLOD(lodActual);
        return (uint)lodActual;
    }

    private void CheckearUpdate()
    {
        int lodActual = _determinarLOD.LOD(transform.position);
        ActualizarLOD(lodActual);
    }

    private void ActualizarLOD(int lodActual)
    {
        if (_lodAnterior != lodActual)
        {
            _actualizar = true;
            _lodAnterior = lodActual;
        }
    }

    public override Dato[] GetDatos()
    {
        return RecalcularDatos();
    }

    private Dato[] RecalcularDatos()
    {
        Vector3Int puntoPorEje = NumeroDePuntosPorEje;
        int cantidadDeDatos = puntoPorEje.x * puntoPorEje.y * puntoPorEje.z;
        Dato[] datos = new Dato[cantidadDeDatos];
        float noiseScale = 0.05f;

        int contador = 0;
        for (int i = 0; i < puntoPorEje.x; i++)
            for (int j = 0; j < puntoPorEje.y; j++)
                for (int k = 0; k < puntoPorEje.z; k++)
                {
                    Vector3 posicionLocal = PosicionLocal(Bounds.size, puntoPorEje, new Vector3Int(i, j, k));
                    Vector3 posicion = posicionLocal + Bounds.center - Bounds.size / 2;
                    
                    Vector3 posicionPerlin = posicion * noiseScale + Vector3.one * 300;
                    float valor = Perlin3D(posicionPerlin);

                    datos[contador++].CargarDatos(posicion, valor);
                }
        return datos;
    }

    private Vector3 PosicionLocal(Vector3 anchoTotal, Vector3Int puntosPorEje, Vector3Int posicion)
    {
        Vector3 posicionFinal = Vector3.zero;
        for (int i = 0; i < 3; i++)
            posicionFinal[i] = Mathf.Lerp(0, anchoTotal[i], ((float)posicion[i]) / (puntosPorEje[i] - 1));
        return posicionFinal;
    }

    private float Perlin3D(Vector3 posicion)
    {
        float x = posicion.x, y = posicion.y, z = posicion.z;

        float XY = Mathf.PerlinNoise(x, y);
        float YZ = Mathf.PerlinNoise(y, z);
        float ZX = Mathf.PerlinNoise(z, x);

        float YX = Mathf.PerlinNoise(y, z);
        float ZY = Mathf.PerlinNoise(z, y);
        float XZ = Mathf.PerlinNoise(x, z);

        return (XY + YZ + ZX + YX + ZY + XZ) / 6f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Bounds.center, Bounds.size);
    }
}
