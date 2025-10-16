using HarmonyLib;

namespace Cobra1982AnimeSoundPatches;

[HarmonyPatch]
public static class Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(NmiPatrouille), nameof(NmiPatrouille.Start))]
    public static void ReplacePatrouilleSound(NmiPatrouille __instance)
    {
        if (__instance.m_SpecificShootProjectileClip != audioSelectionData.eCLIP.NMI_SHOOT_RIFLE)
            return;
        
        if (CobraSoundReplacer.API.CustomSoundUtils.TryGetEClip("1982_anime_NPC_shoot", out var clip))
            __instance.m_SpecificShootProjectileClip = clip;
        else
            Plugin.Logger.LogError("Failed to find EClip for the new shoot sound!");
    }
}