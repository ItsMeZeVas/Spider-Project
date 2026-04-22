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
    public float extendedThreshold = 0.03f;

    [Tooltip("Hace más estricta la detección de puño")]
    public float fistExtraTolerance = 0.015f;

    [Header("Estabilidad")]
    [Tooltip("Tiempo mínimo entre activaciones de cualquier gesto")]
    public float gestureCooldown = 0.25f;

    [Tooltip("Tiempo mínimo para permitir recarga después de un disparo")]
    public float reloadBlockAfterShot = 0.35f;

    [Header("Acción")]
    public WebShooter webShooter;

    [Header("Debug")]
    public bool debugLogs = false;

    private bool spiderGestureWasActive = false;
    private bool fistGestureWasActive = false;
    private bool skeletonReady = false;

    private float lastGestureTime = -999f;
    private float lastShotGestureTime = -999f;

    void Update()
    {
        if (hand == null || skeleton == null || webShooter == null)
            return;

        if (!hand.IsTracked)
            return;

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

        bool cooldownPassed = Time.time >= lastGestureTime + gestureCooldown;
        bool reloadWindowPassed = Time.time >= lastShotGestureTime + reloadBlockAfterShot;

        // RECARGA
        if (fistActive && !fistGestureWasActive && cooldownPassed && reloadWindowPassed)
        {
            if (debugLogs) Debug.Log($"{name}: ✊ RECARGA detectada");
            webShooter.ActivateReloadFromGesture();
            lastGestureTime = Time.time;
        }

        // DISPARO
        if (spiderActive && !spiderGestureWasActive && cooldownPassed)
        {
            if (debugLogs) Debug.Log($"{name}: 🕷️ DISPARO detectado");
            webShooter.ActivateFromGesture();
            lastGestureTime = Time.time;
            lastShotGestureTime = Time.time;
        }

        fistGestureWasActive = fistActive;
        spiderGestureWasActive = spiderActive;
    }

    bool IsSpiderManGesture()
    {
        bool thumbExtended = IsExtended(OVRSkeleton.BoneId.Hand_ThumbTip);
        bool indexExtended = IsExtended(OVRSkeleton.BoneId.Hand_IndexTip);
        bool pinkyExtended = IsExtended(OVRSkeleton.BoneId.Hand_PinkyTip);

        bool middleCurled = IsCurled(OVRSkeleton.BoneId.Hand_MiddleTip);
        bool ringCurled = IsCurled(OVRSkeleton.BoneId.Hand_RingTip);

        return thumbExtended && indexExtended && pinkyExtended && middleCurled && ringCurled;
    }

    bool IsFistGesture()
    {
        bool thumbCurled = IsStrongCurled(OVRSkeleton.BoneId.Hand_ThumbTip);
        bool indexCurled = IsStrongCurled(OVRSkeleton.BoneId.Hand_IndexTip);
        bool middleCurled = IsStrongCurled(OVRSkeleton.BoneId.Hand_MiddleTip);
        bool ringCurled = IsStrongCurled(OVRSkeleton.BoneId.Hand_RingTip);
        bool pinkyCurled = IsStrongCurled(OVRSkeleton.BoneId.Hand_PinkyTip);

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
}