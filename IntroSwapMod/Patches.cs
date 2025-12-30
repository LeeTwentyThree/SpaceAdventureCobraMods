using HarmonyLib;
using UnityEngine;
using UnityEngine.Video;

namespace IntroSwapMod;

[HarmonyPatch]
public static class Patches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(NUIMainMenu), nameof(NUIMainMenu.Start))]
    public static void ReplaceVideoPatch(NUIMainMenu __instance)
    {
        var videoBundleName = Plugin.BundleName.Value;
        if (!Plugin.VideoDatabase.TrySelectVideo(videoBundleName, out var video))
        {
            Plugin.Logger.LogMessage("Using default intro video");
            return;
        }

        if (video == null)
        {
            Plugin.Logger.LogWarning("Failed to find video for bundle name " + videoBundleName);
            return;
        }

        var player = __instance.videoPlayer;
        player.playFromAddressables = false;
        player.videoName = "introswap:" + videoBundleName;
        player.selectAudioTrackFromLanguage = false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CobraVideoPlayer), nameof(CobraVideoPlayer.Play), typeof(string), typeof(GameObject), typeof(Token))]
    public static bool LoadVideoPatch(CobraVideoPlayer __instance, GameObject activateOnEnd, Token OnEndToken)
    {
        const string introSwapPrefix = "introswap:";
        if (!__instance.videoName.StartsWith(introSwapPrefix))
            return true;
        var videoFileName = __instance.videoName.Substring(introSwapPrefix.Length);
        if (!Plugin.VideoDatabase.TrySelectVideo(videoFileName, out var video))
        {
            Plugin.Logger.LogMessage("Using default intro video");
            return true;
        }
        __instance.Play(video, activateOnEnd, OnEndToken);
        return false;
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CobraVideoPlayer), nameof(CobraVideoPlayer.Play), typeof(string), typeof(GameObject), typeof(Token))]
    public static void ChangeVideoVolumePatch(CobraVideoPlayer __instance)
    {
        const string introSwapPrefix = "introswap:";
        if (!__instance.videoName.StartsWith(introSwapPrefix))
            return;
        __instance.player.SetDirectAudioVolume(0, LoadSaveController.Instance.PreferencesData.volumes[5] * LoadSaveController.Instance.PreferencesData.volumes[0] * Plugin.VideoVolume.Value);   
    }
}