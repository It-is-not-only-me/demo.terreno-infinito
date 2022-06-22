using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{
    [SerializeField] private float _rapidez = 10f;


    private void FixedUpdate()
    {
        Vector2 direccion = Direccion();
        Vector3 velocidad = new Vector3(direccion.x, 0, direccion.y) * _rapidez;
        transform.position += velocidad * Time.deltaTime;
    }

    private Vector2 Direccion()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
}
