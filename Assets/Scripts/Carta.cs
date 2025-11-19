using UnityEngine;

public class Carta : MonoBehaviour
{
    public static Carta Instance;

    [TextArea]
    public string mensajeCarta;

    public void RecolectarCarta()
    {
        GameManager.Instance.RecolectarCarta(mensajeCarta);
        Destroy(gameObject);
    }
}
