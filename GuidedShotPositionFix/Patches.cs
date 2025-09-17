using UnityEngine;
using HarmonyLib;

namespace GuidedShotPositionFix;

[HarmonyPatch]
public static class Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CobraCharacter), nameof(CobraCharacter.Start))]
    public static void CobraStartPostfix(CobraCharacter __instance)
    {
        __instance.gameObject.AddComponent<GuidedShotPositionMarker>();
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(GuidedProjectile), nameof(GuidedProjectile.Init))]
    public static void InitPrefix(GuidedProjectile __instance, string authorName, ref Vector3 position)
    {
        if (GuidedShotPositionMarker.TryGetProperShootPosition(authorName, out var overridePosition))
        {
            position = overridePosition;
        }
        else
        {
            Plugin.Logger.LogWarning("Failed to override position!");
        }
    }
}