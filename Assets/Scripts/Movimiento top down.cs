using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimientotopdown : MonoBehaviour 
{
    [SerializeField] private float velocidadMovimiento;

    [SerializeField] private Vector2 direccion;

    private Rigidbody2D rb2D;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        direccion = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }

    private void FixedUpdate()
    {
        rb2D.MovePosition(rb2D.position + direccion * velocidadMovimiento * Time.deltaTime);
    }
}
