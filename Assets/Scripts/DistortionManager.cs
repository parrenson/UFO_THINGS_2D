using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;

public class DistortionManager : MonoBehaviour
{
    public CanvasGroup distortionOverlay;
    public float distortionDuration = 1.5f;
    public float maxAlpha = 0.7f;
    public ParanoiaEffect paranoiaEffect;
    public float resetTimeWindow = 15f;
    private List<float> distortionTimes = new List<float>();
    public Transform playerTransform;
    public Tilemap tilemapMapa; // Asocia tu Tilemap en el Inspector
    public int maxAttempts = 30;
    public float safeRadius = 0.3f; // Ajusta si tu jugador o obstáculos son grandes

    void Start()
    {
        if (distortionOverlay != null)
            distortionOverlay.alpha = 0;
    }

    public void TriggerDistortion()
    {
        StartCoroutine(DoVisualDistortion());
        if (paranoiaEffect != null)
            paranoiaEffect.TriggerShake();
        distortionTimes.Add(Time.time);
        CheckAndHandleDistortionCount();
    }

    private IEnumerator DoVisualDistortion()
    {
        float elapsed = 0f;
        while (elapsed < distortionDuration / 2f)
        {
            elapsed += Time.deltaTime;
            distortionOverlay.alpha = Mathf.Lerp(0, maxAlpha, elapsed / (distortionDuration / 2f));
            yield return null;
        }
        elapsed = 0f;
        while (elapsed < distortionDuration / 2f)
        {
            elapsed += Time.deltaTime;
            distortionOverlay.alpha = Mathf.Lerp(maxAlpha, 0, elapsed / (distortionDuration / 2f));
            yield return null;
        }
        distortionOverlay.alpha = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TriggerDistortion();
        }
    }

    void CheckAndHandleDistortionCount()
    {
        distortionTimes.RemoveAll(t => Time.time - t > resetTimeWindow);
        if (distortionTimes.Count >= 3)
        {
            TeleportPlayerRandomSafe();
            distortionTimes.Clear();
        }
    }

    void TeleportPlayerRandomSafe()
    {
        BoundsInt bounds = tilemapMapa.cellBounds;
        int attempts = 0;
        while (attempts < maxAttempts)
        {
            int x = Random.Range(bounds.xMin, bounds.xMax);
            int y = Random.Range(bounds.yMin, bounds.yMax);
            Vector3Int cell = new Vector3Int(x, y, 0);
            TileBase tile = tilemapMapa.GetTile(cell);
            if (tile == null) { attempts++; continue; }
            Vector3 worldPos = tilemapMapa.CellToWorld(cell) + tilemapMapa.tileAnchor;
            Collider2D[] colls = Physics2D.OverlapCircleAll(new Vector2(worldPos.x + 0.5f, worldPos.y + 0.5f), safeRadius);
            bool blocked = false;
            foreach (var c in colls)
            {
                if (c.CompareTag("Obstaculo")) { blocked = true; break; }
            }
            if (blocked) { attempts++; continue; }
            playerTransform.position = new Vector3(worldPos.x + 0.5f, worldPos.y + 0.5f, playerTransform.position.z);
            return;
        }
        // Si no encuentra posición después del máximo de intentos, no mueve al jugador.
    }
}
