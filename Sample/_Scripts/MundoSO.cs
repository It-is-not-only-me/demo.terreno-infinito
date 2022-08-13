using UnityEngine;
using ItIsNotOnlyMe.SparseOctree;
using ItIsNotOnlyMe.MarchingCubes;
using System.Collections;

[CreateAssetMenu(menuName = "Demo Terreno Infinito/Mundo", fileName = "Mundo")]
public class MundoSO : ScriptableObject
{
    [SerializeField] private Vector3 _posicion, _tamanio;
    [SerializeField] private Vector3Int _puntosPorEje;
    [SerializeField] private int _profunidad;

    [Space]

    [SerializeField] private GenerarDatoSO _generadorDeDatos;

    Octree<Dato> _datos;

    private void OnEnable()
    {
        _datos = new Octree<Dato>(_posicion, _tamanio, _profunidad);
        CargarDatos();
    }

    private void CargarDatos()
    {
        Vector3 desfase = Vector3.zero;
        for (int i = 0; i < 3; i++)
            desfase[i] = _tamanio[i] / _puntosPorEje[i];

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
        for (int i = 0; i < _puntosPorEje.x; i++)
            for (int j = 0; j < _puntosPorEje.x; j++)
                for (int k = 0; k < _puntosPorEje.x; k++)
                {
                    posicionRelativa.Set(i, j, k);
                    yield return posicionRelativa;
                }
    }
}
