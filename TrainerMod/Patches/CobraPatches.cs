using HarmonyLib;

namespace TrainerMod.Patches;

[HarmonyPatch(typeof(CobraCharacter))]
public static class CobraPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(CobraCharacter.Start))]
    public static void CobraStartPostfix(CobraCharacter __instance)
    {
        __instance.gameObject.AddComponent<TrainerStatManager>().cobra = __instance;
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(CobraCharacter), nameof(CobraCharacter.ShootChargedShot))]
    public static void TrackFullyChargedShots(CobraCharacter __instance)
    {
        if (__instance.timeSinceMelee < __instance.melee.noShootAfterMeleeDelay || !__instance.dependencies.amICobra)
        {
            return;
        }

        bool fullyCharged = __instance.chargedShotHoldTimer > __instance.aiming.chargedShotTime;
        FullyChargedShotTracker.OnShoot(fullyCharged);
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(CobraCharacter.isUltraAvailable), MethodType.Getter)]
    public static void ForceUltraAvailable(ref bool __result)
    {
        if (Plugin.UnlimitedSuperShots.Value)
        {
            __result = true;
        }
    }
}