using HarmonyLib;

namespace ChargedShotsBreakShields;

[HarmonyPatch]
public static class Patches
{
    private static audioSelectionData.eCLIP _penetratingShotSound;

    private static bool _overrideNextShootSub;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CobraCharacter), nameof(CobraCharacter.Start))]
    public static void CobraStartPostfix(CobraCharacter __instance)
    {
        __instance.dependencies.chargedShot.AddComponent<DestroyShieldBehavior>();
        if (!CobraSoundReplacer.API.CustomSoundUtils.TryGetEClip("penetrating_shot", out _penetratingShotSound))
        {
            Plugin.Logger.LogWarning("Failed to find penetrating shot sound");
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CobraCharacter), nameof(CobraCharacter.ShootChargedShot))]
    public static void OnChargedShotShoot(CobraCharacter __instance)
    {
        if (__instance.timeSinceMelee < __instance.melee.noShootAfterMeleeDelay || !__instance.dependencies.amICobra)
        {
            return;
        }

        bool fullyCharged = __instance.chargedShotHoldTimer > __instance.aiming.chargedShotTime;
        FullyChargedShotTracker.OnShoot(fullyCharged);
        if (fullyCharged)
        {
            _overrideNextShootSub = true;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AudioController), nameof(AudioController.PlaySound), typeof(audioSelectionData.eCLIP),
        typeof(float), typeof(byte), typeof(byte), typeof(float))]
    private static void UsePenetratingPsychoGunSoundPatch(ref audioSelectionData.eCLIP _clip)
    {
        if (_overrideNextShootSub && _clip == audioSelectionData.eCLIP.PLY_SHOOT_PG_CHARGEDLAUNCHED)
        {
            _clip = _penetratingShotSound;
            _overrideNextShootSub = false;
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AudioController), nameof(AudioController.PlaySound), typeof(audioSelectionData.eCLIP),
        typeof(float), typeof(byte), typeof(byte), typeof(float))]
    private static void PenetratingShotMixerFixPatch(ref audioSelectionData.eCLIP _clip, CAudio.CPlayingAudioData __result)
    {
        if (_clip != _penetratingShotSound)
            return;
        if (__result.asrc == null)
            return;
        
        Plugin.LoudSoundMixerGroup.audioMixer.SetFloat("_VolumeGain", Plugin.PenetratingShotDecibelGain.Value);
        __result.asrc.outputAudioMixerGroup = Plugin.LoudSoundMixerGroup;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CobraCharacter), nameof(CobraCharacter.ShootSub))]
    private static void ShootSubPostfix()
    {
        _overrideNextShootSub = false;
    }
}