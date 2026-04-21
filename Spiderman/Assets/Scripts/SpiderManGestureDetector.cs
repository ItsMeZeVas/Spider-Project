using UnityEngine;

public class SpiderManGestureDetector : MonoBehaviour
{
    [Header("Referencias de mano")]
    public OVRHand hand;
    public OVRSkeleton skeleton;

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

    [Header("Acción")]
    public WebShooter webShooter;

    // ── estado ────────────────────────────────────────────────────────────────
    private bool gestureWasActive = false;

    void Update()
    {
        if (!hand.IsTracked) return;

        bool gestureActive = IsSpiderManGesture();

        // Solo dispara en el FLANCO DE SUBIDA (cuando entra en la pose)
        if (gestureActive && !gestureWasActive)
            webShooter?.ActivateFromGesture();

        gestureWasActive = gestureActive;
    }

    bool IsSpiderManGesture()
    {
        bool indexExtended = GetDistance(OVRSkeleton.BoneId.Hand_Index3) >= indexExtendedThreshold;
        bool pinkyExtended = GetDistance(OVRSkeleton.BoneId.Hand_Pinky3) >= pinkyExtendedThreshold;
        bool middleCurled = GetDistance(OVRSkeleton.BoneId.Hand_Middle3) < middleCurledThreshold;
        bool ringCurled = GetDistance(OVRSkeleton.BoneId.Hand_Ring3) < ringCurledThreshold;
        bool thumbCurled = GetDistance(OVRSkeleton.BoneId.Hand_ThumbTip) < thumbCurledThreshold;

        return indexExtended && pinkyExtended && middleCurled && ringCurled && thumbCurled;
    }

    float GetDistance(OVRSkeleton.BoneId tipId)
    {
        OVRBone tip = GetBone(tipId);
        OVRBone wrist = GetBone(OVRSkeleton.BoneId.Hand_WristRoot);
        if (tip == null || wrist == null) return 0f;
        return Vector3.Distance(tip.Transform.position, wrist.Transform.position);
    }

    OVRBone GetBone(OVRSkeleton.BoneId id)
    {
        foreach (var bone in skeleton.Bones)
            if (bone.Id == id) return bone;
        return null;
    }

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
}