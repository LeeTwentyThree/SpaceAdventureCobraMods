using CobraSoundReplacer.Core;
using HarmonyLib;

namespace CobraSoundReplacer.Patches;

[HarmonyPatch(typeof(audioSelectionData))]
public static class AudioSelectionDataPatcher
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(audioSelectionData.getSoundName))]
    public static bool GetSoundNamePrefix(audioSelectionData.eCLIP myClip, ref string __result)
    {
        if (SoundPackRegistry.CustomEClipSounds.TryGetValue(myClip, out var clip))
        {
            __result = clip;
            return false;
        }
        return true;
    }
}