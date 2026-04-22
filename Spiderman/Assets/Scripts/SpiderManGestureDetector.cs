using UnityEngine;

public class SpiderManGestureDetector : MonoBehaviour
{
    [Header("Referencias de mano")]
    public OVRHand hand;
    public OVRSkeleton skeleton;

    [Header("Configuración")]
    [Tooltip("Distancia menor a esto = dedo doblado")]
    public float curledThreshold = 0.06f;

    [Tooltip("Distancia mayor o igual a esto = dedo extendido")]
    public float extendedThreshold = 0.11f;

    [Tooltip("Hace más estricta la detección de puño")]
    public float fistExtraTolerance = 0.02f;

    [Header("Acción")]
    public WebShooter webShooter;

    [Header("Debug")]
    public bool debugLogs = false;

    private bool spiderGestureWasActive = false;
    private bool fistGestureWasActive = false;
    private bool skeletonReady = false;

    void Update()
    {
        if (hand == null || skeleton == null || webShooter == null)
            return;

        if (!hand.IsTracked)
            return;

        // Esperar a que el skeleton esté listo
        if (!skeletonReady)
        {
            if (skeleton.IsInitialized)
            {
                skeletonReady = true;
                if (debugLogs) Debug.Log($"{name}: Skeleton listo");
            }
            else
            {
                return;
            }
        }

        bool fistActive = IsFistGesture();
        bool spiderActive = false;

        // Solo detectar disparo si NO es puño
        if (!fistActive)
            spiderActive = IsSpiderManGesture();

        // Detectar transición (no spam)
        if (fistActive && !fistGestureWasActive)
        {
            if (debugLogs) Debug.Log($"{name}: ✊ RECARGA detectada");
            webShooter.ActivateReloadFromGesture();
        }

        if (spiderActive && !spiderGestureWasActive)
        {
            if (debugLogs) Debug.Log($"{name}: 🕷️ DISPARO detectado");
            webShooter.ActivateFromGesture();
        }

        fistGestureWasActive = fistActive;
        spiderGestureWasActive = spiderActive;
    }

    // 🕷️ Gesto Spider-Man
    bool IsSpiderManGesture()
    {
        bool thumbExtended = IsExtended(OVRSkeleton.BoneId.Hand_ThumbTip);
        bool indexExtended = IsExtended(OVRSkeleton.BoneId.Hand_IndexTip);
        bool pinkyExtended = IsExtended(OVRSkeleton.BoneId.Hand_PinkyTip);

        bool middleCurled = IsCurled(OVRSkeleton.BoneId.Hand_MiddleTip);
        bool ringCurled = IsCurled(OVRSkeleton.BoneId.Hand_RingTip);

        if (debugLogs)
        {
            Debug.Log($"Spider -> Thumb:{thumbExtended} Index:{indexExtended} Pinky:{pinkyExtended} Mid:{middleCurled} Ring:{ringCurled}");
        }

        return thumbExtended && indexExtended && pinkyExtended && middleCurled && ringCurled;
    }

    // ✊ Gesto puño
    bool IsFistGesture()
    {
        bool thumbCurled = IsStrongCurled(OVRSkeleton.BoneId.Hand_ThumbTip);
        bool indexCurled = IsStrongCurled(OVRSkeleton.BoneId.Hand_IndexTip);
        bool middleCurled = IsStrongCurled(OVRSkeleton.BoneId.Hand_MiddleTip);
        bool ringCurled = IsStrongCurled(OVRSkeleton.BoneId.Hand_RingTip);
        bool pinkyCurled = IsStrongCurled(OVRSkeleton.BoneId.Hand_PinkyTip);

        if (debugLogs)
        {
            Debug.Log($"Fist -> T:{thumbCurled} I:{indexCurled} M:{middleCurled} R:{ringCurled} P:{pinkyCurled}");
        }

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
            OVRSkeleton.BoneId.Hand_IndexTip,
            OVRSkeleton.BoneId.Hand_MiddleTip,
            OVRSkeleton.BoneId.Hand_RingTip,
            OVRSkeleton.BoneId.Hand_PinkyTip
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