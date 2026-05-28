using UnityEngine;

public class SpiderManGestureDetector : MonoBehaviour
{
    [Header("Referencias de mano")]
    public OVRHand hand;
    public OVRSkeleton skeleton;

    [Header("Acción")]
    public WebShooter webShooter;

    [Header("Configuración")]

    [Tooltip("Distancia para dedo doblado")]
    public float curledThreshold = 0.055f;

    [Tooltip("Distancia para dedo extendido")]
    public float extendedThreshold = 0.075f;

    [Tooltip("Más estricto para puño")]
    public float fistExtraTolerance = 0.015f;

    [Header("Estabilidad")]

    [Tooltip("Tiempo entre activaciones")]
    public float gestureCooldown = 0.25f;

    [Tooltip("Bloqueo de recarga después de disparo")]
    public float reloadBlockAfterShot = 0.35f;

    [Tooltip("Tiempo que debe mantenerse el gesto")]
    public float requiredHoldTime = 0.08f;

    [Tooltip("Delay inicial")]
    public float startupDelay = 1f;

    [Header("Debug")]
    public bool debugLogs = false;

    private bool skeletonReady = false;

    private bool spiderGestureActive = false;
    private bool fistGestureActive = false;

    private float spiderGestureStartTime;
    private float fistGestureStartTime;

    private float lastGestureTime = -999f;
    private float lastShotGestureTime = -999f;

    private bool detectionEnabled = false;

    void Start()
    {
        Invoke(nameof(EnableDetection), startupDelay);
    }

    void EnableDetection()
    {
        detectionEnabled = true;

        if (debugLogs)
            Debug.Log($"{name}: Detección activada");
    }

    void Update()
    {
        if (!detectionEnabled)
            return;

        if (hand == null || skeleton == null || webShooter == null)
            return;

        if (!hand.IsTracked)
            return;

        if (hand.HandConfidence == OVRHand.TrackingConfidence.Low)
            return;

        //--------------------------------
        // Esperar skeleton
        //--------------------------------

        if (!skeletonReady)
        {
            if (skeleton.IsInitialized)
            {
                skeletonReady = true;

                if (debugLogs)
                    Debug.Log($"{name}: Skeleton listo");
            }
            else
            {
                return;
            }
        }

        //--------------------------------
        // Gestos
        //--------------------------------

        bool fistDetected = IsFistGesture();

        bool spiderDetected = false;

        // Evitar disparo si es puño
        if (!fistDetected)
            spiderDetected = IsSpiderManGesture();

        bool cooldownPassed =
            Time.time >= lastGestureTime + gestureCooldown;

        bool reloadWindowPassed =
            Time.time >= lastShotGestureTime + reloadBlockAfterShot;

        //--------------------------------
        // DISPARO
        //--------------------------------

        if (spiderDetected)
        {
            if (!spiderGestureActive)
            {
                spiderGestureActive = true;
                spiderGestureStartTime = Time.time;
            }

            bool holdPassed =
                Time.time >= spiderGestureStartTime + requiredHoldTime;

            if (holdPassed && cooldownPassed)
            {
                if (debugLogs)
                    Debug.Log($"{name}: 🕷️ DISPARO");

                webShooter.ActivateFromGesture();

                lastGestureTime = Time.time;
                lastShotGestureTime = Time.time;

                spiderGestureStartTime = Time.time + 999f;
            }
        }
        else
        {
            spiderGestureActive = false;
        }

        //--------------------------------
        // RECARGA
        //--------------------------------

        if (fistDetected)
        {
            if (!fistGestureActive)
            {
                fistGestureActive = true;
                fistGestureStartTime = Time.time;
            }

            bool holdPassed =
                Time.time >= fistGestureStartTime + requiredHoldTime;

            if (holdPassed &&
                cooldownPassed &&
                reloadWindowPassed)
            {
                if (debugLogs)
                    Debug.Log($"{name}: ✊ RECARGA");

                webShooter.ActivateReloadFromGesture();

                lastGestureTime = Time.time;

                fistGestureStartTime = Time.time + 999f;
            }
        }
        else
        {
            fistGestureActive = false;
        }
    }

    //--------------------------------
    // GESTO SPIDERMAN
    //--------------------------------

    bool IsSpiderManGesture()
    {
        bool indexExtended =
            IsExtended(OVRSkeleton.BoneId.Hand_IndexTip);

        bool pinkyExtended =
            IsExtended(OVRSkeleton.BoneId.Hand_PinkyTip);

        bool middleCurled =
            IsCurled(OVRSkeleton.BoneId.Hand_MiddleTip);

        bool ringCurled =
            IsCurled(OVRSkeleton.BoneId.Hand_RingTip);

        // Pulgar ignorado para estabilidad

        return
            indexExtended &&
            pinkyExtended &&
            middleCurled &&
            ringCurled;
    }

    //--------------------------------
    // GESTO PUÑO
    //--------------------------------

    bool IsFistGesture()
    {
        bool thumbCurled =
            IsStrongCurled(OVRSkeleton.BoneId.Hand_ThumbTip);

        bool indexCurled =
            IsStrongCurled(OVRSkeleton.BoneId.Hand_IndexTip);

        bool middleCurled =
            IsStrongCurled(OVRSkeleton.BoneId.Hand_MiddleTip);

        bool ringCurled =
            IsStrongCurled(OVRSkeleton.BoneId.Hand_RingTip);

        bool pinkyCurled =
            IsStrongCurled(OVRSkeleton.BoneId.Hand_PinkyTip);

        return
            thumbCurled &&
            indexCurled &&
            middleCurled &&
            ringCurled &&
            pinkyCurled;
    }

    //--------------------------------
    // DEDO EXTENDIDO
    //--------------------------------

    bool IsExtended(OVRSkeleton.BoneId tipId)
    {
        OVRBone tip = GetBone(tipId);
        OVRBone wrist = GetBone(OVRSkeleton.BoneId.Hand_WristRoot);

        if (tip == null || wrist == null)
            return false;

        float distance =
            Vector3.Distance(
                tip.Transform.position,
                wrist.Transform.position
            );

        return distance >= extendedThreshold;
    }

    //--------------------------------
    // DEDO DOBLADO
    //--------------------------------

    bool IsCurled(OVRSkeleton.BoneId tipId)
    {
        OVRBone tip = GetBone(tipId);
        OVRBone wrist = GetBone(OVRSkeleton.BoneId.Hand_WristRoot);

        if (tip == null || wrist == null)
            return false;

        float distance =
            Vector3.Distance(
                tip.Transform.position,
                wrist.Transform.position
            );

        return distance <= curledThreshold;
    }

    //--------------------------------
    // DEDO MUY DOBLADO
    //--------------------------------

    bool IsStrongCurled(OVRSkeleton.BoneId tipId)
    {
        OVRBone tip = GetBone(tipId);
        OVRBone wrist = GetBone(OVRSkeleton.BoneId.Hand_WristRoot);

        if (tip == null || wrist == null)
            return false;

        float distance =
            Vector3.Distance(
                tip.Transform.position,
                wrist.Transform.position
            );

        return distance <=
               (curledThreshold - fistExtraTolerance);
    }

    //--------------------------------
    // Obtener hueso
    //--------------------------------

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