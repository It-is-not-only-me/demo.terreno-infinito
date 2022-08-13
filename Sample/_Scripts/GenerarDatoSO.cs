using UnityEngine;

public abstract class GenerarDatoSO : ScriptableObject, IGenerarDato
{
    public abstract float GenerarValor(Vector3 posicion);
}
