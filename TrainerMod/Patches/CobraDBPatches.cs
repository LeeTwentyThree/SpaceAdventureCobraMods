using HarmonyLib;
using TrainerMod.Enums;

namespace TrainerMod.Patches;

[HarmonyPatch(typeof(CobraDB))]
public static class CobraDBPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(CobraDB.GetStat), typeof(CobraDB.Stats), typeof(int))]
    public static void GetStatPostfix(ref float __result, CobraDB.Stats stats)
    {
        switch (stats)
        {
            case CobraDB.Stats.Life:
                if (ShouldOverrideStat(Plugin.CobraHealth.Value))
                    __result = Plugin.CobraHealth.Value;
                break;
            case CobraDB.Stats.ChargedPsychogunDamage:
                if (FullyChargedShotTracker.GetDidPlayerShootFullyChargedShot() && ShouldOverrideStat(Plugin.PsychogunPenetratingShotDamage.Value))
                    __result = Plugin.PsychogunPenetratingShotDamage.Value;
                else if (ShouldOverrideStat(Plugin.PsychogunDamage.Value))
                    __result = Plugin.PsychogunDamage.Value;
                break;
            case CobraDB.Stats.GuidedShotDamage:
                if (ShouldOverrideStat(Plugin.CurvedShotDamage.Value))
                    __result = Plugin.CurvedShotDamage.Value;
                break;
            case CobraDB.Stats.GuidedShotNbCharge:
                if (Plugin.CurvedShotMode.Value == GuidedShotMode.SingleUse)
                {
                    __result = 1;
                }
                break;
        }
    }

    private static bool ShouldOverrideStat(float value)
    {
        return value >= 0f;
    }
}