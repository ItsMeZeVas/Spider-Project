using System.Collections;
using UnityEngine;

public class SpiderManGestureDetector : MonoBehaviour
{
    [Header("Referencias de mano")]
    public OVRHand hand;
    public OVRSkeleton skeleton;

    [Header("Configuración")]
    [Tooltip("Distancia máxima punta-muñeca para considerar dedo DOBLADO")]
    public float curledThreshold = 0.07f;

    [Tooltip("Distancia mínima punta-muñeca para considerar dedo ESTIRADO")]
    public float extendedThreshold = 0.09f;

    [Tooltip("Segundos entre detecciones")]
    public float cooldown = 0.8f;

    [Header("Acción")]
    public WebShooter webShooter;

    private float lastGestureTime = -999f;

    void Update()
    {
        if (!hand.IsTracked) return;
        if (Time.time - lastGestureTime < cooldown) return;

        if (IsSpiderManGesture())
        {
            lastGestureTime = Time.time;
            webShooter?.ActivateFromGesture();
        }
    }

    bool IsSpiderManGesture()
    {
        // Índice estirado  ✓
        bool indexExtended = IsExtended(OVRSkeleton.BoneId.Hand_Index3);
        // Meñique estirado ✓
        bool pinkyExtended = IsExtended(OVRSkeleton.BoneId.Hand_Pinky3);
        // Medio doblado    ✓
        bool middleCurled = IsCurled(OVRSkeleton.BoneId.Hand_Middle3);
        // Anular doblado   ✓
        bool ringCurled = IsCurled(OVRSkeleton.BoneId.Hand_Ring3);
        // Pulgar doblado   ✓
        bool thumbCurled = IsCurled(OVRSkeleton.BoneId.Hand_ThumbTip);

        return indexExtended && pinkyExtended && middleCurled && ringCurled && thumbCurled;
    }

    bool IsExtended(OVRSkeleton.BoneId tipId)
    {
        OVRBone tip = GetBone(tipId);
        OVRBone wrist = GetBone(OVRSkeleton.BoneId.Hand_WristRoot);
        if (tip == null || wrist == null) return false;
        return Vector3.Distance(tip.Transform.position, wrist.Transform.position) >= extendedThreshold;
    }

    bool IsCurled(OVRSkeleton.BoneId tipId)
    {
        OVRBone tip = GetBone(tipId);
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
    // Ayuda visual en consola para calibrar los umbrales
    [ContextMenu("Debug distancias de dedos")]
    void DebugDistances()
    {
        if (skeleton == null) { Debug.Log("Skeleton no asignado"); return; }
        OVRSkeleton.BoneId[] tips = {
            OVRSkeleton.BoneId.Hand_Index3,
            OVRSkeleton.BoneId.Hand_Middle3,
            OVRSkeleton.BoneId.Hand_Ring3,
            OVRSkeleton.BoneId.Hand_Pinky3,
            OVRSkeleton.BoneId.Hand_ThumbTip
        };
        string[] names = { "Índice", "Medio", "Anular", "Meñique", "Pulgar" };
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