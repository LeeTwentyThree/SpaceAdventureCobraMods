using HarmonyLib;
using UnityEngine;

namespace MinimalHUD;

[HarmonyPatch]
public static class Patches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(UIController), nameof(UIController.Awake))]
    public static void ModifyUIElements(UIController __instance)
    {
        __instance.uiDirectionPrefab.AddComponent<CanvasGroup>().ignoreParentGroups = true;
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(UIController), nameof(UIController.AddGuidedShotTarget))]
    public static bool RemoveGuidedShotTarget()
    {
        return false;
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(NUIHUD), nameof(NUIHUD.Start))]
    public static void HideHUD(NUIHUD __instance)
    {
        var canvasGroup = __instance.gameObject.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }
}