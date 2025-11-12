using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int salud = 100;
    public int saludMax = 100;

    public void Curar(int cantidad)
    {
        salud += cantidad;
        if (salud > saludMax)
            salud = saludMax;

        Debug.Log("Curado. Salud actual: " + salud);
    }
}
