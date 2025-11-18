using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoaderUI : MonoBehaviour
{
    public GameObject loaderPanel;      // Asigna tu panel de carga desde el Inspector
    public float waitSeconds = 1.0f;    // Duración de la espera antes de cambiar de escena

    void Start()
    {
        if (loaderPanel != null)
            loaderPanel.SetActive(false); // Panel oculto al iniciar
    }

    public void LoadSceneWithDelay(string sceneName)
    {
        StartCoroutine(ShowLoaderAndChangeScene(sceneName));
    }

    IEnumerator ShowLoaderAndChangeScene(string sceneName)
    {
        if (loaderPanel != null)
            loaderPanel.SetActive(true);

        yield return new WaitForSeconds(waitSeconds);

        SceneManager.LoadScene(sceneName);
    }
}
