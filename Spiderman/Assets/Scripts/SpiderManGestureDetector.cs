using UnityEngine;

public class SpiderManGestureDetector : MonoBehaviour
{
    [Header("Referencias de mano")]
    public OVRHand hand;
    public OVRSkeleton skeleton;

    [Header("Configuración")]
    [Tooltip("Distancia menor a esto = dedo doblado")]
    public float curledThreshold = 0.07f;

    [Tooltip("Distancia mayor o igual a esto = dedo extendido")]
    public float extendedThreshold = 0.09f;

    [Tooltip("Hace más estricta la detección de puño para evitar que se active por error")]
    public float fistExtraTolerance = 0.015f;

    [Header("Acción")]
    public WebShooter webShooter;

    [Header("Debug")]
    public bool debugLogs = false;

    private bool spiderGestureWasActive = false;
    private bool fistGestureWasActive = false;

    void Update()
    {
        if (hand == null || skeleton == null || webShooter == null)
            return;

        if (!hand.IsTracked || skeleton.Bones == null || skeleton.Bones.Count == 0)
            return;

        bool fistActive = IsFistGesture();
        bool spiderActive = false;

        // Prioridad: si es puño, no evaluar disparo
        if (!fistActive)
            spiderActive = IsSpiderManGesture();

        if (fistActive && !fistGestureWasActive)
        {
            if (debugLogs) Debug.Log($"{name}: RECARGA detectada");
            webShooter.ActivateReloadFromGesture();
        }

        if (spiderActive && !spiderGestureWasActive)
        {
            if (debugLogs) Debug.Log($"{name}: DISPARO detectado");
            webShooter.ActivateFromGesture();
        }

        fistGestureWasActive = fistActive;
        spiderGestureWasActive = spiderActive;
    }

    bool IsSpiderManGesture()
    {
        bool thumbExtended = IsExtended(OVRSkeleton.BoneId.Hand_ThumbTip);
        bool indexExtended = IsExtended(OVRSkeleton.BoneId.Hand_Index3);
        bool pinkyExtended = IsExtended(OVRSkeleton.BoneId.Hand_Pinky3);

        bool middleCurled = IsCurled(OVRSkeleton.BoneId.Hand_Middle3);
        bool ringCurled = IsCurled(OVRSkeleton.BoneId.Hand_Ring3);

        return thumbExtended && indexExtended && pinkyExtended && middleCurled && ringCurled;
    }

    bool IsFistGesture()
    {
        bool thumbCurled = IsStrongCurled(OVRSkeleton.BoneId.Hand_ThumbTip);
        bool indexCurled = IsStrongCurled(OVRSkeleton.BoneId.Hand_Index3);
        bool middleCurled = IsStrongCurled(OVRSkeleton.BoneId.Hand_Middle3);
        bool ringCurled = IsStrongCurled(OVRSkeleton.BoneId.Hand_Ring3);
        bool pinkyCurled = IsStrongCurled(OVRSkeleton.BoneId.Hand_Pinky3);

        return thumbCurled && indexCurled && middleCurled && ringCurled && pinkyCurled;
    }

    bool IsExtended(OVRSkeleton.BoneId tipId)
    {
        OVRBone tip = GetBone(tipId);
        OVRBone wrist = GetBone(OVRSkeleton.BoneId.Hand_WristRoot);

        if (tip == null || wrist == null)
            return false;

        float distance = Vector3.Distance(tip.Transform.position, wrist.Transform.position);
        return distance >= extendedThreshold;
    }

    bool IsCurled(OVRSkeleton.BoneId tipId)
    {
        OVRBone tip = GetBone(tipId);
        OVRBone wrist = GetBone(OVRSkeleton.BoneId.Hand_WristRoot);

        if (tip == null || wrist == null)
            return false;

        float distance = Vector3.Distance(tip.Transform.position, wrist.Transform.position);
        return distance < curledThreshold;
    }

    bool IsStrongCurled(OVRSkeleton.BoneId tipId)
    {
        OVRBone tip = GetBone(tipId);
        OVRBone wrist = GetBone(OVRSkeleton.BoneId.Hand_WristRoot);

        if (tip == null || wrist == null)
            return false;

        float distance = Vector3.Distance(tip.Transform.position, wrist.Transform.position);
        return distance < (curledThreshold - fistExtraTolerance);
    }

    OVRBone GetBone(OVRSkeleton.BoneId id)
    {
        foreach (var bone in skeleton.Bones)
        {
            if (bone.Id == id)
                return bone;
        }

        return null;
    }

#if UNITY_EDITOR
    [ContextMenu("Debug distancias de dedos")]
    void DebugDistances()
    {
        if (skeleton == null)
        {
            Debug.Log("Skeleton no asignado");
            return;
        }

        OVRSkeleton.BoneId[] tips =
        {
            OVRSkeleton.BoneId.Hand_ThumbTip,
            OVRSkeleton.BoneId.Hand_Index3,
            OVRSkeleton.BoneId.Hand_Middle3,
            OVRSkeleton.BoneId.Hand_Ring3,
            OVRSkeleton.BoneId.Hand_Pinky3
        };

        string[] names = { "Pulgar", "Índice", "Medio", "Anular", "Meñique" };

        OVRBone wrist = GetBone(OVRSkeleton.BoneId.Hand_WristRoot);

        for (int i = 0; i < tips.Length; i++)
        {
            OVRBone tip = GetBone(tips[i]);

            if (tip != null && wrist != null)
            {
                float dist = Vector3.Distance(tip.Transform.position, wrist.Transform.position);
                Debug.Log($"{name} | {names[i]}: {dist:F4}m");
            }
        }
    }
#endif
}