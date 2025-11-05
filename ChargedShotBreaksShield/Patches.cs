using HarmonyLib;

namespace ChargedShotsBreakShields;

[HarmonyPatch]
public static class Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CobraCharacter), nameof(CobraCharacter.Start))]
    public static void CobraStartPostfix(CobraCharacter __instance)
    {
        __instance.dependencies.chargedShot.AddComponent<DestroyShieldBehavior>();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CobraCharacter), nameof(CobraCharacter.ShootChargedShot))]
    public static void OnChargedShotShoot(CobraCharacter __instance)
    {
        if (__instance.timeSinceMelee < __instance.melee.noShootAfterMeleeDelay || !__instance.dependencies.amICobra)
        {
            return;
        }

        FullyChargedShotTracker.OnShoot(__instance.chargedShotHoldTimer > __instance.aiming.chargedShotTime);
    }
}