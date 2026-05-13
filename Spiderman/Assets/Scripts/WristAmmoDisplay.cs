using UnityEngine;

public class WristAmmoDisplay : MonoBehaviour
{
    [Header("Referencias mando")]
    public Transform wristTransform;

    [Header("Referencias hand tracking")]
    public OVRSkeleton skeleton;

    [Header("Shooter y display")]
    public GameObject displayRoot;
    public WebShooter webShooter;

    [Header("Posición")]
    public Vector3 positionOffset = new Vector3(0f, 0.05f, 0f);
    public Vector3 rotationOffset = new Vector3(0f, 0f, 0f);

    [Header("Cuadrícula")]
    public int columnas = 4;
    public float espaciadoX = 0.015f;
    public float espaciadoY = 0.015f;
    public float tamano = 0.008f;

    [Header("Colores")]
    public Color colorLlena = new Color(0.5f, 0.8f, 1f);
    public Color colorVacia = new Color(0.2f, 0.2f, 0.2f);

    private GameObject[] esferas;
    private Material[] materiales;
    private OVRBone wristBone;

    void Start()
    {
        CrearEsferas();
    }

    Transform GetWrist()
    {
        // Intentar hand tracking primero
        if (skeleton != null && skeleton.IsInitialized &&
            skeleton.Bones != null && skeleton.Bones.Count > 0)
        {
            if (wristBone == null)
            {
                foreach (var bone in skeleton.Bones)
                {
                    if (bone.Id == OVRSkeleton.BoneId.Hand_WristRoot)
                    {
                        wristBone = bone;
                        break;
                    }
                }
            }

            if (wristBone != null)
                return wristBone.Transform;
        }

        // Si no hay hand tracking usar el anchor del mando
        return wristTransform;
    }

    void CrearEsferas()
    {
        int total = webShooter != null ? webShooter.maxAmmo : 7;
        esferas = new GameObject[total];
        materiales = new Material[total];

        int cols = Mathf.Max(1, columnas);
        int filas = Mathf.CeilToInt((float)total / cols);

        for (int i = 0; i < total; i++)
        {
            int col = i % cols;
            int fila = i / cols;

            float offsetX = (col - (cols - 1) / 2f) * espaciadoX;
            float offsetY = (fila - (filas - 1) / 2f) * espaciadoY * -1f;

            GameObject esfera = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            esfera.transform.SetParent(displayRoot.transform);
            esfera.transform.localScale = Vector3.one * tamano;
            esfera.transform.localPosition = new Vector3(offsetX, offsetY, 0f);
            esfera.transform.localRotation = Quaternion.identity;

            Destroy(esfera.GetComponent<Collider>());

            Material mat = new Material(Shader.Find("Sprites/Default"));
            mat.color = colorLlena;
            esfera.GetComponent<Renderer>().material = mat;

            esferas[i] = esfera;
            materiales[i] = mat;
        }
    }

    void Update()
    {
        Transform wrist = GetWrist();

        if (wrist == null || displayRoot == null) return;

        displayRoot.transform.position = wrist.position +
            wrist.TransformDirection(positionOffset);
        displayRoot.transform.rotation = wrist.rotation *
            Quaternion.Euler(rotationOffset);

        if (webShooter == null || esferas == null) return;

        int ammo = webShooter.currentAmmo;
        for (int i = 0; i < esferas.Length; i++)
            materiales[i].color = i < ammo ? colorLlena : colorVacia;
    }
}