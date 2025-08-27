using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace NoFramerateCap;

[HarmonyPatch]
public static class Patches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(FrameRateLimiter), nameof(FrameRateLimiter.Start))]
    public static void OverrideFramerateCapPatch(FrameRateLimiter __instance)
    {
        __instance.targetFrameRate = CustomFramerateUtils.GetNewFramerateInt();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(QualityController), nameof(QualityController.Update))]
    public static void OverrideFrameRateCapInUpdate()
    {
        Application.targetFrameRate = CustomFramerateUtils.GetNewFramerateInt();
        if (Plugin.DisableVsync.Value)
            QualitySettings.vSyncCount = 0;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CFrame), nameof(CFrame.reset))]
    public static void CFrameResetPostfix()
    {
        CFrame.fps = (ushort)CustomFramerateUtils.GetNewFramerateInt();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ProjectileRocket), nameof(ProjectileRocket.RotateTowards))]
    public static bool RotateTowardsReplacement(ProjectileRocket __instance, in Vector3 direction, in Vector3 idealDirection, in float degPerFrame, in float frameDuration, ref Vector3 __result)
    {
        __result = Vector3.RotateTowards(direction, idealDirection, degPerFrame * frameDuration * (MathF.PI / 180f) * CustomFramerateUtils.GetNewFramerateFloat() / 60, 0f);
        return false;
    }
}

[HarmonyPatch(typeof(NmiPatrouille), nameof(NmiPatrouille.Update))]
public static class NmiPatrouilleTimeDependencePatch
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return TranspilerUtils.ReplaceSixty(instructions);
    }
}

[HarmonyPatch(typeof(ProjectileRocket), nameof(ProjectileRocket.futureCurve))]
public static class RocketProjectilePatch
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        bool firstFourFound = false;
        foreach (var code in instructions)
        {
            if (code.opcode == OpCodes.Ldc_R4 && code.operand is float operand && Mathf.Approximately(operand, 0.016666668f))
            {
                code.operand = 1f / CustomFramerateUtils.GetNewFramerateFloat();
                yield return code;
            }
            else
            {
                yield return code;
            }
        }
    }
}

public static class TranspilerUtils
{
    public static IEnumerable<CodeInstruction> ReplaceSixty(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var code in instructions)
        {
            if (code.opcode == OpCodes.Ldc_R4 && code.operand is float operandFloat && Mathf.Approximately(operandFloat, 60f))
            {
                code.operand = CustomFramerateUtils.GetNewFramerateFloat();
                yield return code;
            }
            else if ((code.opcode == OpCodes.Ldc_I4 || code.opcode == OpCodes.Ldc_I4_S) && code.operand is int operandInt && operandInt == 60)
            {
                code.operand = CustomFramerateUtils.GetNewFramerateInt();
                yield return code;
            }
            else
            {
                yield return code;
            }
        }
    }
}