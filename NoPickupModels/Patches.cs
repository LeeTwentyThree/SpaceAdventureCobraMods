using HarmonyLib;
using UnityEngine;

namespace NoPickupModels;

[HarmonyPatch]
public static class Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(NmiDropCollectible), nameof(NmiDropCollectible.Awake))]
    public static void AwakePostfix(NmiDropCollectible __instance)
    {
        foreach (var renderer in __instance.GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(CobraCharacter), nameof(CobraCharacter.DropPickedup))]
    public static bool DisablePickUpEffect()
    {
        return false;
    }
}