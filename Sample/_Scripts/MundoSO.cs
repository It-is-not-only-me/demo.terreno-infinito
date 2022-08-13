using UnityEngine;
using ItIsNotOnlyMe.SparseOctree;
using ItIsNotOnlyMe.MarchingCubes;
using System.Collections;

[CreateAssetMenu(menuName = "Demo Terreno Infinito/Mundo", fileName = "Mundo")]
public class MundoSO : ScriptableObject
{
    [SerializeField] private Vector3 _posicion, _tamanio;
    [SerializeField] private int _puntosPorEje;

    [Space]

    [SerializeField] private GenerarDatoSO _generadorDeDatos;

    Octree<Dato> _datos;

    private void OnEnable()
    {
        int profunidad = CalcularProfunidad();
        Debug.Log(profunidad);
        _datos = new Octree<Dato>(_posicion, _tamanio, profunidad);
        CargarDatos();
    }

    private int CalcularProfunidad()
    {
        int profunidad = 0;
        
        for (int nodosVisibles = 1; nodosVisibles < _puntosPorEje; nodosVisibles *= 2)
            profunidad++;

        return profunidad;
    }

    private void CargarDatos()
    {
        Vector3 desfase = Vector3.zero;
        for (int i = 0; i < 3; i++)
            desfase[i] = _tamanio[i] / _puntosPorEje;

        foreach (Vector3Int posicionRelativa in ObtenerPosicionesRelativas())
        {
            Vector3 posicion = Vector3.zero;
            for (int i = 0; i < 3; i++)
                posicion[i] = _posicion[i] + posicionRelativa[i] * desfase[i];

            float valor = _generadorDeDatos.GenerarValor(posicion);
            _datos.Insertar(posicion, new Dato(posicion, valor));
        }
    }

    private IEnumerable ObtenerPosicionesRelativas()
    {
        Vector3Int posicionRelativa = Vector3Int.zero;
        for (int i = 0; i < _puntosPorEje; i++)
            for (int j = 0; j < _puntosPorEje; j++)
                for (int k = 0; k < _puntosPorEje; k++)
                {
                    posicionRelativa.Set(i, j, k);
                    yield return posicionRelativa;
                }
    }
}
