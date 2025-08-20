using CobraSoundReplacer.Core;
using HarmonyLib;

namespace CobraSoundReplacer.Patches;

[HarmonyPatch(typeof(CAudio))]
public static class CAudioPatcher
{
    [HarmonyPatch(nameof(CAudio.Init))]
    [HarmonyPostfix]
    public static void InitPatch(CAudio __instance)
    {
        Plugin.StartCoroutineOnPlugin(SoundPackRegistry.OnAudioInitialized(__instance));
    }
}