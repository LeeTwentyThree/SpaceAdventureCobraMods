using HarmonyLib;
using UnityEngine.Audio;

namespace Cobra1982AnimeSoundPatches;

[HarmonyPatch]
public static class Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(NmiPatrouille), nameof(NmiPatrouille.Start))]
    public static void ReplacePatrouilleSound(NmiPatrouille __instance)
    {
        if (__instance.m_SpecificShootProjectileClip == audioSelectionData.eCLIP.NMI_SHOOT_RIFLE)
        {
            if (__instance.gameObject.name.StartsWith("NmiCrystalBoy_Light_ShotBasic"))
            {
                if (CobraSoundReplacer.API.CustomSoundUtils.TryGetEClip("1982_anime_NPC_shoot", out var clip))
                    __instance.m_SpecificShootProjectileClip = clip;
                else
                    Plugin.Logger.LogError("Failed to find EClip for the new shoot sound!");
            }
        }
        else if (__instance.m_SpecificShootProjectileClip == audioSelectionData.eCLIP.NMI_SHOOT_MULTI)
        {
            if (__instance.gameObject.name.StartsWith("NmiCrystalBoy_Elite_ShotSpread"))
            {
                if (CobraSoundReplacer.API.CustomSoundUtils.TryGetEClip("1982_anime_NPC_multi_shoot", out var clip))
                    __instance.m_SpecificShootProjectileClip = clip;
                else
                    Plugin.Logger.LogError("Failed to find EClip for the new multi shoot sound!");
            }
        }
    }

    private const int PsychogunChargeSlsIndex = 0;
    private const float AudioPlayDuration = 0.98f;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AudioController), nameof(AudioController.UpdateSingleLoopSounds))]
    public static void ForcePsychogunChargeSoundToPlayFullyPatch(AudioController __instance)
    {
        if (__instance.m_SLS_UsedCount[PsychogunChargeSlsIndex] <= 0)
            return;

        if (__instance.m_SLS_PAD[PsychogunChargeSlsIndex] == null)
            return;

        var chargeSound = __instance.m_SLS_PAD[PsychogunChargeSlsIndex];

        if (chargeSound.asrc != null && chargeSound.asrc.clip != null &&
            chargeSound.asrc.time < chargeSound.asrc.clip.length * AudioPlayDuration)
        {
            __instance.m_SLS_UsedCount[PsychogunChargeSlsIndex] = 2;
        }
    }
}