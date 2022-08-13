using UnityEngine;

[CreateAssetMenu(menuName = "Demo Terreno Infinito/Generar datos Perlin SO", fileName = "Generar datos Perlin SO")]
public class GenerarDatoPerlinNoiseSO : GenerarDatoSO
{
    [SerializeField] float _escala, _cercaniaALaSuperficie;
    
    public override float GenerarValor(Vector3 posicion)
    {
        return GenerarValor(posicion.x, posicion.y, posicion.z);
    }

    private float GenerarValor(float x, float y, float z)
    {
        float valorPerlin = Mathf.PerlinNoise(x * _escala, z * _escala);
        return Mathf.InverseLerp(y - _cercaniaALaSuperficie, y + _cercaniaALaSuperficie, valorPerlin);
    }
}