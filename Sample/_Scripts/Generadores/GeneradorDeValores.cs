using UnityEngine;

public abstract class GeneradorDeValores : MonoBehaviour, IGeneradorDeValores
{
    public float Valor(Vector3 posicion)
    {
        return Valor(posicion.x, posicion.y, posicion.z);   
    }
    public abstract float Valor(float i, float j, float k);
}
