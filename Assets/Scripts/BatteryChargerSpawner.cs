using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class BatteryChargerSpawner : MonoBehaviour
{
    [Header("Prefab del cargador de batería")]
    public GameObject batteryChargerPrefab;

    [Header("Cantidad de cargadores a generar")]
    public int cantidadCargadores = 10;

    [Header("Referencia al Tilemap del terreno")]
    public Tilemap terrenoTilemap;

    [Header("Altura fija en Z")]
    public float spawnZ = 0f;

    [Header("Objetos que bloquean generación")]
    public List<Collider2D> obstaculos = new List<Collider2D>();

    [Header("Margen de seguridad (distancia mínima al obstáculo)")]
    public float margen = 0.5f;

    [Header("Distancia mínima entre cargadores")]
    public float distanciaMinimaEntreCargadores = 2f;

    private List<Vector3> posicionesGeneradas = new List<Vector3>();

    void Start()
    {
        GenerarCargadores();
    }

    void GenerarCargadores()
    {
        if (batteryChargerPrefab == null || terrenoTilemap == null)
        {
            Debug.LogError("⚠️ Asigna el prefab y el tilemap en el inspector.");
            return;
        }

        BoundsInt bounds = terrenoTilemap.cellBounds;
        int generados = 0;
        int intentos = 0;

        while (generados < cantidadCargadores && intentos < cantidadCargadores * 30)
        {
            intentos++;

            float x = Random.Range(bounds.xMin, bounds.xMax);
            float y = Random.Range(bounds.yMin, bounds.yMax);
            Vector3 worldPos = terrenoTilemap.CellToWorld(new Vector3Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y), 0));
            worldPos += new Vector3(0.5f, 0.5f, spawnZ);

            if (PosicionValida(worldPos))
            {
                Instantiate(batteryChargerPrefab, worldPos, Quaternion.identity);
                posicionesGeneradas.Add(worldPos);
                generados++;
            }
        }

        Debug.Log($"🔋 Generados {generados} cargadores (de {cantidadCargadores}) en {intentos} intentos.");
    }

    bool PosicionValida(Vector3 pos)
    {
        // Evita obstáculos
        foreach (var obs in obstaculos)
        {
            if (obs == null) continue;

            Bounds b = obs.bounds;
            float distancia = Vector2.Distance(pos, b.ClosestPoint(pos));

            if (distancia < margen)
                return false;
        }

        // Evita estar muy cerca de otros cargadores
        foreach (var existente in posicionesGeneradas)
        {
            if (Vector2.Distance(pos, existente) < distanciaMinimaEntreCargadores)
                return false;
        }

        return true;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (terrenoTilemap != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(terrenoTilemap.cellBounds.center, terrenoTilemap.cellBounds.size);
        }
    }
#endif
}
