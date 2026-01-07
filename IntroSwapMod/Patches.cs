using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
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
        if (Plugin.FitVertically.Value)
            __instance.player.aspectRatio = VideoAspectRatio.FitVertically;
        CreateBlackBackground(__instance.player);
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

    private static void CreateBlackBackground(VideoPlayer player)
    {
        var canvas = GameObject.Find("ui_Canvas").GetComponent<Canvas>();
        var imgObject = new GameObject("BlackBackgroundImage");
        var imgRect = imgObject.AddComponent<RectTransform>();
        imgRect.SetParent(canvas.GetComponent<RectTransform>());
        imgRect.anchorMin = Vector2.zero;
        imgRect.anchorMax = Vector2.one;
        imgRect.localPosition = Vector3.zero;
        imgRect.localScale = Vector3.one;
        var img = imgObject.AddComponent<Image>();
        img.color = Color.black;
        imgRect.SetSiblingIndex(2);
        var barsDestroyer = new GameObject("BlackBarsDestroyer").AddComponent<DestroyVideoBlackBars>();
        barsDestroyer.bars = imgObject;
        barsDestroyer.player = player;
    }
}