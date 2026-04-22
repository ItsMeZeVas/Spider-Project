using UnityEngine;

public class SpiderManGestureDetector : MonoBehaviour
{
    [Header("Referencias de mano")]
    public OVRHand hand;
    public OVRSkeleton skeleton;

<<<<<<< HEAD
    [Header("Configuración")]
    public float curledThreshold = 0.07f;
    public float extendedThreshold = 0.09f;
=======
    [Header("Dedos ESTIRADOS (índice y meñique)")]
    [Tooltip("Distancia mínima para considerar el índice estirado")]
    public float indexExtendedThreshold = 0.09f;
    [Tooltip("Distancia mínima para considerar el meñique estirado")]
    public float pinkyExtendedThreshold = 0.09f;

    [Header("Dedos DOBLADOS (medio, anular, pulgar)")]
    [Tooltip("Distancia máxima para considerar el medio doblado")]
    public float middleCurledThreshold = 0.07f;
    [Tooltip("Distancia máxima para considerar el anular doblado")]
    public float ringCurledThreshold = 0.07f;
    [Tooltip("Distancia máxima para considerar el pulgar doblado")]
    public float thumbCurledThreshold = 0.07f;
>>>>>>> 9d22e1b99e2e53a6d4827277ccd54c48a95c0e12

    [Header("Acción")]
    public WebShooter webShooter;

<<<<<<< HEAD
    private bool spiderGestureWasActive = false;
    private bool fistGestureWasActive = false;
=======
    // ── estado ────────────────────────────────────────────────────────────────
    private bool gestureWasActive = false;
>>>>>>> 9d22e1b99e2e53a6d4827277ccd54c48a95c0e12

    void Update()
    {
        if (hand == null || skeleton == null || webShooter == null)
            return;

        if (!hand.IsTracked || skeleton.Bones == null || skeleton.Bones.Count == 0)
            return;

<<<<<<< HEAD
        bool fistActive = IsFistGesture();
        bool spiderActive = false;

        // Prioridad: si es puño, no evaluar disparo
        if (!fistActive)
            spiderActive = IsSpiderManGesture();

        // Recarga: solo al entrar en el gesto
        if (fistActive && !fistGestureWasActive)
        {
            webShooter.ActivateReloadFromGesture();
        }
=======
        // Solo dispara en el FLANCO DE SUBIDA (cuando entra en la pose)
        if (gestureActive && !gestureWasActive)
            webShooter?.ActivateFromGesture();
>>>>>>> 9d22e1b99e2e53a6d4827277ccd54c48a95c0e12

        // Disparo: solo al entrar en el gesto
        if (spiderActive && !spiderGestureWasActive)
        {
            webShooter.ActivateFromGesture();
        }

        fistGestureWasActive = fistActive;
        spiderGestureWasActive = spiderActive;
    }

    bool IsSpiderManGesture()
    {
<<<<<<< HEAD
        bool thumbExtended = IsExtended(OVRSkeleton.BoneId.Hand_ThumbTip);
        bool indexExtended = IsExtended(OVRSkeleton.BoneId.Hand_Index3);
        bool pinkyExtended = IsExtended(OVRSkeleton.BoneId.Hand_Pinky3);
        bool middleCurled = IsCurled(OVRSkeleton.BoneId.Hand_Middle3);
        bool ringCurled = IsCurled(OVRSkeleton.BoneId.Hand_Ring3);
=======
        bool indexExtended = GetDistance(OVRSkeleton.BoneId.Hand_Index3) >= indexExtendedThreshold;
        bool pinkyExtended = GetDistance(OVRSkeleton.BoneId.Hand_Pinky3) >= pinkyExtendedThreshold;
        bool middleCurled = GetDistance(OVRSkeleton.BoneId.Hand_Middle3) < middleCurledThreshold;
        bool ringCurled = GetDistance(OVRSkeleton.BoneId.Hand_Ring3) < ringCurledThreshold;
        bool thumbCurled = GetDistance(OVRSkeleton.BoneId.Hand_ThumbTip) < thumbCurledThreshold;
>>>>>>> 9d22e1b99e2e53a6d4827277ccd54c48a95c0e12

        return thumbExtended && indexExtended && pinkyExtended && middleCurled && ringCurled;
    }

    bool IsFistGesture()
    {
        bool thumbCurled = IsCurled(OVRSkeleton.BoneId.Hand_ThumbTip);
        bool indexCurled = IsCurled(OVRSkeleton.BoneId.Hand_Index3);
        bool middleCurled = IsCurled(OVRSkeleton.BoneId.Hand_Middle3);
        bool ringCurled = IsCurled(OVRSkeleton.BoneId.Hand_Ring3);
        bool pinkyCurled = IsCurled(OVRSkeleton.BoneId.Hand_Pinky3);

        return thumbCurled && indexCurled && middleCurled && ringCurled && pinkyCurled;
    }

    float GetDistance(OVRSkeleton.BoneId tipId)
    {
        OVRBone tip = GetBone(tipId);
        OVRBone wrist = GetBone(OVRSkeleton.BoneId.Hand_WristRoot);
<<<<<<< HEAD

        if (tip == null || wrist == null)
            return false;

        return Vector3.Distance(tip.Transform.position, wrist.Transform.position) >= extendedThreshold;
    }

    bool IsCurled(OVRSkeleton.BoneId tipId)
    {
        OVRBone tip = GetBone(tipId);
        OVRBone wrist = GetBone(OVRSkeleton.BoneId.Hand_WristRoot);

        if (tip == null || wrist == null)
            return false;

        return Vector3.Distance(tip.Transform.position, wrist.Transform.position) < curledThreshold;
=======
        if (tip == null || wrist == null) return 0f;
        return Vector3.Distance(tip.Transform.position, wrist.Transform.position);
>>>>>>> 9d22e1b99e2e53a6d4827277ccd54c48a95c0e12
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
<<<<<<< HEAD
=======

#if UNITY_EDITOR
    [ContextMenu("Debug distancias de dedos")]
    void DebugDistances()
    {
        if (skeleton == null) { Debug.Log("Skeleton no asignado"); return; }

        var tips = new[] {
            (OVRSkeleton.BoneId.Hand_Index3,   "Índice  "),
            (OVRSkeleton.BoneId.Hand_Middle3,  "Medio   "),
            (OVRSkeleton.BoneId.Hand_Ring3,    "Anular  "),
            (OVRSkeleton.BoneId.Hand_Pinky3,   "Meñique "),
            (OVRSkeleton.BoneId.Hand_ThumbTip, "Pulgar  "),
        };

        foreach (var (id, name) in tips)
            Debug.Log($"{name}: {GetDistance(id):F4}m");
    }
#endif
>>>>>>> 9d22e1b99e2e53a6d4827277ccd54c48a95c0e12
}