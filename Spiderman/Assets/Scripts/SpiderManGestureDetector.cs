using UnityEngine;

public class SpiderManGestureDetector : MonoBehaviour
{
    [Header("Referencias de mano")]
    public OVRHand hand;
    public OVRSkeleton skeleton;

    [Header("Configuración")]
    public float curledThreshold   = 0.07f;
    public float extendedThreshold = 0.09f;

    [Header("Acción")]
    public WebShooter webShooter;

    // ── estado ────────────────────────────────────────────────────────────────
    private bool gestureWasActive = false;  // pose activa el frame anterior

    void Update()
    {
        if (!hand.IsTracked) return;

        bool gestureActive = IsSpiderManGesture();

        // Solo dispara en el FLANCO DE SUBIDA (cuando entra en la pose)
        if (gestureActive && !gestureWasActive)
        {
            webShooter?.ActivateFromGesture();
        }

        gestureWasActive = gestureActive;
    }

    bool IsSpiderManGesture()
    {
        bool indexExtended  = IsExtended(OVRSkeleton.BoneId.Hand_Index3);
        bool pinkyExtended  = IsExtended(OVRSkeleton.BoneId.Hand_Pinky3);
        bool middleCurled   = IsCurled(OVRSkeleton.BoneId.Hand_Middle3);
        bool ringCurled     = IsCurled(OVRSkeleton.BoneId.Hand_Ring3);
        bool thumbCurled    = IsCurled(OVRSkeleton.BoneId.Hand_ThumbTip);

        return indexExtended && pinkyExtended && middleCurled && ringCurled && thumbCurled;
    }

    bool IsExtended(OVRSkeleton.BoneId tipId)
    {
        OVRBone tip   = GetBone(tipId);
        OVRBone wrist = GetBone(OVRSkeleton.BoneId.Hand_WristRoot);
        if (tip == null || wrist == null) return false;
        return Vector3.Distance(tip.Transform.position, wrist.Transform.position) >= extendedThreshold;
    }

    bool IsCurled(OVRSkeleton.BoneId tipId)
    {
        OVRBone tip   = GetBone(tipId);
        OVRBone wrist = GetBone(OVRSkeleton.BoneId.Hand_WristRoot);
        if (tip == null || wrist == null) return false;
        return Vector3.Distance(tip.Transform.position, wrist.Transform.position) < curledThreshold;
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
        OVRSkeleton.BoneId[] tips  = { OVRSkeleton.BoneId.Hand_Index3, OVRSkeleton.BoneId.Hand_Middle3, OVRSkeleton.BoneId.Hand_Ring3, OVRSkeleton.BoneId.Hand_Pinky3, OVRSkeleton.BoneId.Hand_ThumbTip };
        string[]             names = { "Índice", "Medio", "Anular", "Meñique", "Pulgar" };
        OVRBone wrist = GetBone(OVRSkeleton.BoneId.Hand_WristRoot);
        for (int i = 0; i < tips.Length; i++)
        {
            OVRBone tip = GetBone(tips[i]);
            if (tip != null && wrist != null)
                Debug.Log($"{names[i]}: {Vector3.Distance(tip.Transform.position, wrist.Transform.position):F4}m");
        }
    }
#endif
}