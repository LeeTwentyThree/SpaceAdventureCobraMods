using HarmonyLib;
using UnityEngine;
using UnityEngine.Video;

namespace CutsceneVolumeFix;

[HarmonyPatch]
public static class Patches
{
    private static bool _overriding;
    private static float _lastMusicVolume;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CobraVideoPlayer), nameof(CobraVideoPlayer.Play), typeof(VideoClip), typeof(GameObject), typeof(Token))]
    public static void OnCutsceneStart()
    {
        if (!_overriding)
        {
            _lastMusicVolume = LoadSaveController.Instance.PreferencesData.volumes[2];
            _overriding = true;
        }
        
        var sfxVolume = AudioController.Instance.mixageGal[0];
        AudioController.Instance.SetMusicGlobalVolume(sfxVolume);
        LoadSaveController.Instance.PreferencesData.volumes[2] = sfxVolume;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CobraVideoPlayer), nameof(CobraVideoPlayer.Stop))]
    public static void OnCutsceneStop()
    {
        if (!_overriding)
            return;
        AudioController.Instance.SetMusicGlobalVolume(_lastMusicVolume);
        LoadSaveController.Instance.PreferencesData.volumes[2] = _lastMusicVolume;
        _overriding = false;
    }
}