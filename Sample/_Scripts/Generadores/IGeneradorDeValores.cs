using UnityEngine;

public interface IGeneradorDeValores
{
    public float Valor(Vector3 posicion);

    public float Valor(float i, float j, float k);
}
