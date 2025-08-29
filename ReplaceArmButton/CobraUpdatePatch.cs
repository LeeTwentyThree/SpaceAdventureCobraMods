using HarmonyLib;
using UnityEngine;

namespace ReplaceArmButton;

[HarmonyPatch(typeof(CobraCharacter), nameof(CobraCharacter.Update))]
public static class CobraUpdatePatch
{
    private static readonly int TForceProtheseOn = Animator.StringToHash("t_forceProtheseOn");
    private static readonly int ProtheseOnHash = Animator.StringToHash("b_isProtheseOn");
    
    private const float DPadThreshold = 0.5f;

    [HarmonyPostfix]
    public static void Postfix(CobraCharacter __instance)
    {
        if (__instance.animator.GetBool(ProtheseOnHash))
            return;

        if (Input.GetKeyDown(Plugin.KeyboardBinding.Value) || GetDPadButton(Plugin.ControllerDPadOption.Value))
        {
            __instance.animator.SetTrigger(TForceProtheseOn);
        }
    }

    private static bool GetDPadButton(DPadDirection dpad)
    {
        float horizontal = Input.GetAxis("6th axis"); // X
        float vertical   = Input.GetAxis("7th axis"); // Y

        switch (dpad)
        {
            case DPadDirection.Up:
                return vertical > DPadThreshold;
            case DPadDirection.Down:
                return vertical < -DPadThreshold;
            case DPadDirection.Left:
                return horizontal < -DPadThreshold;
            case DPadDirection.Right:
                return horizontal > DPadThreshold;
            default:
                Plugin.Logger.LogWarning("Invalid DPadDirection: " + dpad);
                return false;
        }
    }
}