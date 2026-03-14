using HarmonyLib;

namespace MusicReplacer.ReplacementSystem;

[HarmonyPatch(typeof(NUIMainMenu))]
public static class MusicProcessorEntry
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(NUIMainMenu.Start))]
    public static void Init()
    {
        MusicProcessor.ScrapeAllData(AudioController.Audio);
    }
}