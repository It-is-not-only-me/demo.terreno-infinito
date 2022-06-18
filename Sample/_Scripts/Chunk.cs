using System.Collections;
using UnityEngine;
using ItIsNotOnlyMe.MarchingCubes;

/*
 * La intencion de esta clase que sin importar el nivel de detalle
 * que sea capaz de representar bien su contendio usando marching cubes
 */
public class Chunk : GenerarDatos
{

    public override Bounds Bounds 
    {
        get
        {
            if (_bounds == null)
                _bounds = new Bounds(transform.position, _radio);
            return _bounds;
        }
    }

    public override Vector3Int NumeroDePuntosPorEje => _configuracionLOD.numeroDePuntosPorNivelDeDetalle[LOD()];

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
    [SerializeField] private Vector3 _radio;

    private DeterminarLOD _determinarLOD;
    private Bounds _bounds;
    private int _lodAnterior;
    private bool _actualizar;

    private void Awake()
    {
        if (!TryGetComponent(out _determinarLOD))
            Debug.LogError("Se necesita una forma de determinar el nivel de detalle");
        _lodAnterior = -1;
        _actualizar = true;
    }

    private int LOD()
    {
        int lodActual = _determinarLOD.LOD(transform.position);
        ActualizarLOD(lodActual);
        return lodActual;
    }

    private void Update()
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
                    float x = Mathf.Lerp(0, _bounds.size.x * 2, ((float)i) / (puntoPorEje.x - 1));
                    float y = Mathf.Lerp(0, _bounds.size.y * 2, ((float)j) / (puntoPorEje.y - 1));
                    float z = Mathf.Lerp(0, _bounds.size.z * 2, ((float)k) / (puntoPorEje.z - 1));

                    Vector3 posicion = new Vector3(x, y, z) + _bounds.center - _bounds.size;
                    float valor = Mathf.PerlinNoise(posicion.x * noiseScale, posicion.z * noiseScale);
                    valor *= 20 - j;
                    datos[contador++].CargarDatos(posicion, valor);
                }
        return datos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Bounds.center, _radio * 2);
    }
}
