using HarmonyLib;

namespace NoSuperShotPopup;

[HarmonyPatch]
public static class Patches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(UIController), nameof(UIController.UIShowNotificationUltraEnergy))]
    public static bool RemoveUltraEnergyNotification(ref bool __result)
    {
        __result = true;
        return false;
    }
}