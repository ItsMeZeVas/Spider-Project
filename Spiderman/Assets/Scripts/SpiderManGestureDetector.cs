using UnityEngine;

public class SpiderManGestureDetector : MonoBehaviour
{
    [Header("Referencias de mano")]
    public OVRHand hand;
    public OVRSkeleton skeleton;

    [Header("Configuración")]
    public float curledThreshold = 0.07f;
    public float extendedThreshold = 0.09f;

    [Header("Acción")]
    public WebShooter webShooter;

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

        // Recarga: solo al entrar en el gesto
        if (fistActive && !fistGestureWasActive)
        {
            webShooter.ActivateReloadFromGesture();
        }

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
        bool thumbExtended = IsExtended(OVRSkeleton.BoneId.Hand_ThumbTip);
        bool indexExtended = IsExtended(OVRSkeleton.BoneId.Hand_Index3);
        bool pinkyExtended = IsExtended(OVRSkeleton.BoneId.Hand_Pinky3);
        bool middleCurled = IsCurled(OVRSkeleton.BoneId.Hand_Middle3);
        bool ringCurled = IsCurled(OVRSkeleton.BoneId.Hand_Ring3);

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

    bool IsExtended(OVRSkeleton.BoneId tipId)
    {
        OVRBone tip = GetBone(tipId);
        OVRBone wrist = GetBone(OVRSkeleton.BoneId.Hand_WristRoot);

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