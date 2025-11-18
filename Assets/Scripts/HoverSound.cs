using UnityEngine;
using UnityEngine.EventSystems;

public class HoverSound : MonoBehaviour, IPointerEnterHandler
{
    public AudioClip hoverClip;
    public float volume = 1f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverClip != null)
            AudioSource.PlayClipAtPoint(hoverClip, Camera.main.transform.position, volume);
    }
}
