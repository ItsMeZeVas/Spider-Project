using UnityEngine;

public class WebShooterAimIndicatorTutorial : MonoBehaviour
{
    [Header("Referencias")]
    public Transform shootPoint;
    public WebShooterTutorial webShooterTutorial;

    [Header("Línea")]
    public float rayLength = 10f;
    public Color lineColor = new Color(0.5f, 0.8f, 1f, 0.6f);
    public float lineWidth = 0.005f;

    [Header("Indicador de impacto")]
    public GameObject impactIndicatorPrefab;

    private LineRenderer lineRenderer;
    private GameObject impactInstance;

    void Start()
    {
        // Crear el Line Renderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth * 0.5f;
        lineRenderer.useWorldSpace = true;

        // Material simple
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = new Color(lineColor.r, lineColor.g, lineColor.b, 0f);

        // Crear indicador de impacto si no hay prefab
        if (impactIndicatorPrefab != null)
            impactInstance = Instantiate(impactIndicatorPrefab);
        else
            impactInstance = CreateDefaultIndicator();
    }

    void Update()
    {
        if (shootPoint == null) return;

        // Solo mostrar si hay munición
        bool hasAmmo = webShooterTutorial != null && webShooterTutorial.currentAmmo > 0;
        lineRenderer.enabled = hasAmmo;
        impactInstance.SetActive(hasAmmo);

        if (!hasAmmo) return;

        Vector3 origin = shootPoint.position;
        Vector3 direction = shootPoint.forward;

        // Raycast para detectar dónde impacta
        if (Physics.Raycast(origin, direction, out RaycastHit hit, rayLength))
        {
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, hit.point);
            impactInstance.transform.position = hit.point;
            impactInstance.transform.rotation = Quaternion.LookRotation(hit.normal);
        }
        else
        {
            Vector3 endPoint = origin + direction * rayLength;
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, endPoint);
            impactInstance.transform.position = endPoint;
        }
    }

    GameObject CreateDefaultIndicator()
    {
        // Círculo simple como indicador
        GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        indicator.transform.localScale = Vector3.one * 0.05f;
        Destroy(indicator.GetComponent<Collider>());

        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = lineColor;
        indicator.GetComponent<Renderer>().material = mat;

        return indicator;
    }

    void OnDestroy()
    {
        if (impactInstance != null)
            Destroy(impactInstance);
    }
}