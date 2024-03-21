using Unity.VisualScripting;
using UnityEngine;
public static class VectorExtensionMethods {
    public static bool IsZero(this Vector2 v, float thresh = Vector2.kEpsilon) {
        return v.sqrMagnitude <= thresh;
    }
    public static bool IsZero(this Vector3 v, float thresh = Vector3.kEpsilon) {
        return v.sqrMagnitude <= thresh;
    }
}