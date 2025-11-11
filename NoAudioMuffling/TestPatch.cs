using HarmonyLib;

namespace NoAudioMuffling;

[HarmonyPatch]
public static class TestPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(CAudio), nameof(CAudio.play), typeof(string), typeof(float), typeof(float), typeof(CAudio.eVolumeType))]
    public static void TestPrefix(string clipName, ref CAudio.eVolumeType forceType)
    {
        if (!string.IsNullOrEmpty(clipName) && clipName.StartsWith("vox_SYS_BARKS_DAMAGE"))
        {
            forceType = CAudio.eVolumeType.sound;
        }
    }
}