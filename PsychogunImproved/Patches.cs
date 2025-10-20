using System.IO;
using HarmonyLib;
using UnityEngine;

namespace PsychogunImproved;

[HarmonyPatch]
public static class Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CobraCharacter), nameof(CobraCharacter.Start))]
    public static void StartPostfix(CobraCharacter __instance)
    {
        var behaviour = __instance.gameObject.AddComponent<ImprovedPsychogunBehaviour>();

        var armAnimation = __instance.gameObject.AddComponent<RuntimeAdditiveAnimation>();
        armAnimation.LoadFromJSON(
            Path.Combine(Path.GetDirectoryName(Plugin.ModAssembly.Location),
                "RecoilAnimation.json"),
            __instance.transform);
        armAnimation.transitionDuration = 0.1f;
        
        behaviour.animation = armAnimation;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CobraCharacter), nameof(CobraCharacter.ShootPsychogun))]
    public static bool TestPatch(CobraCharacter __instance, Vector2 dir)
    {
        var behaviour = __instance.gameObject.GetComponent<ImprovedPsychogunBehaviour>();
        if (behaviour == null)
        {
            Plugin.Logger.LogWarning("Failed to find ImprovedPsychogunBehaviour!");
            return true;
        }

        if (behaviour.GetCanShoot())
        {
            __instance.ShootChargedShot(dir);
            behaviour.StartShotCooldown();
        }

        return false;
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CobraCharacter), nameof(CobraCharacter.ShootChargedShot))]
    public static void ShootChargedShotPatch(CobraCharacter __instance)
    {
        var behaviour = __instance.gameObject.GetComponent<ImprovedPsychogunBehaviour>();
        if (behaviour == null)
        {
            Plugin.Logger.LogWarning("Failed to find ImprovedPsychogunBehaviour!");
            return;
        }

        behaviour.PlayShootAnimation();
    }
}