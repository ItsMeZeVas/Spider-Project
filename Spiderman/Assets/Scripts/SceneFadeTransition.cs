using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFadeTransition : MonoBehaviour
{
    [Header("Fade VR - Quad frente a la cámara")]
    public float fadeDuration = 1.5f;
    public bool fadeInOnStart = true;

    private GameObject fadeQuad;
    private Material fadeMaterial;

    void Awake()
    {
        CrearFadeQuad();
    }

    void Start()
    {
        if (fadeInOnStart)
            StartCoroutine(FadeIn());
    }

    void CrearFadeQuad()
    {
        // Crear quad hijo de la cámara principal
        Camera cam = Camera.main;

        fadeQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        fadeQuad.name = "VR_FadeQuad";
        fadeQuad.transform.SetParent(cam.transform, false);

        // Posicionarlo justo frente a la cámara, dentro del near clip
        fadeQuad.transform.localPosition = new Vector3(0, 0, cam.nearClipPlane + 0.01f);
        fadeQuad.transform.localRotation = Quaternion.identity;

        // Escalarlo para cubrir todo el campo de visión
        float h = 2f * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad)
                     * (cam.nearClipPlane + 0.01f);
        float w = h * cam.aspect;
        fadeQuad.transform.localScale = new Vector3(w, h, 1f);

        // Quitar el collider
        Destroy(fadeQuad.GetComponent<Collider>());

        // Material con transparencia, que NO haga ZTest (se pinta sobre todo)
        fadeMaterial = new Material(Shader.Find("Sprites/Default"));
        fadeMaterial.color = new Color(0, 0, 0, 0);

        // Renderizar sobre todo lo demás
        fadeMaterial.renderQueue = 4999;
        fadeQuad.GetComponent<Renderer>().material = fadeMaterial;

        // Asegurarse que se vea en ambos ojos
        fadeQuad.layer = cam.gameObject.layer;

        fadeQuad.SetActive(false);
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    IEnumerator FadeIn()
    {
        fadeQuad.SetActive(true);
        yield return StartCoroutine(AnimarAlpha(1f, 0f));
        fadeQuad.SetActive(false);
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        fadeQuad.SetActive(true);
        yield return StartCoroutine(AnimarAlpha(0f, 1f));
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator AnimarAlpha(float desde, float hasta)
    {
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / fadeDuration);
            float alpha = Mathf.Lerp(desde, hasta, t);
            fadeMaterial.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeMaterial.color = new Color(0, 0, 0, hasta);
    }
}